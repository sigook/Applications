import { spawnSync } from 'node:child_process'
import { existsSync, mkdirSync, readFileSync, writeFileSync } from 'node:fs'
import path from 'node:path'
import { parseArgs } from 'node:util'
import libCoverage from 'istanbul-lib-coverage'
import libReport from 'istanbul-lib-report'
import reports from 'istanbul-reports'

const root = process.cwd()
const coverageDir = path.join(root, 'coverage')
const vitestOutput = path.join(coverageDir, 'coverage-final.json')
const baselineOutput = path.join(coverageDir, 'coverage-baseline.json')
const thresholds = JSON.parse(readFileSync(path.join(root, 'test-thresholds.json'), 'utf8'))
const vitestReporters = [
  '--reporter=default',
  '--reporter=junit',
  '--outputFile.junit=test-results/junit.xml',
]
const fullRunTriggers = [
  /^package\.json$/,
  /^pnpm-(lock|workspace)\.yaml$/,
  /^vite(st)?\.config\.ts$/,
  /^tsconfig.*\.json$/,
  /^test-thresholds\.json$/,
  /^scripts\//,
  /^src\/testing\//,
]

const { values: args } = parseArgs({
  options: {
    base: { type: 'string', default: '' },
    baseline: { type: 'string', default: '' },
    full: { type: 'boolean', default: false },
  },
})

const toRelative = (file) => path.relative(root, file).replaceAll('\\', '/')

const readCoverage = (file) =>
  Object.fromEntries(
    Object.values(JSON.parse(readFileSync(file, 'utf8'))).map((data) => {
      const relative = toRelative(path.resolve(root, data.path))
      return [relative, { ...data, path: relative }]
    }),
  )

const writeBaseline = (coverage) => {
  mkdirSync(coverageDir, { recursive: true })
  writeFileSync(baselineOutput, JSON.stringify(coverage))
}

const vitest = (vitestArgs, env = {}) =>
  spawnSync('pnpm', ['exec', 'vitest', ...vitestArgs], {
    stdio: 'inherit',
    shell: process.platform === 'win32',
    env: { ...process.env, ...env },
  }).status ?? 1

const changedFiles = (base) => {
  const diff = spawnSync('git', ['diff', '--name-only', '--relative', base, 'HEAD'], {
    encoding: 'utf8',
  })
  if (diff.status !== 0) return null
  return diff.stdout
    .split('\n')
    .map((line) => line.trim())
    .filter(Boolean)
}

const sourceOf = (file) => {
  const spec = /^(.*)\.spec\.ts$/.exec(file)
  if (!spec) return file
  return (
    ['.ts', '.vue']
      .map((extension) => spec[1] + extension)
      .find((candidate) => existsSync(candidate)) ?? file
  )
}

const runFull = (reason) => {
  console.log(`Full run: ${reason}`)
  const status = vitest(['run', '--coverage', ...vitestReporters])
  if (status !== 0) return status
  writeBaseline(readCoverage(vitestOutput))
  return 0
}

const checkThresholds = (coverage) => {
  const coverageMap = libCoverage.createCoverageMap(
    Object.fromEntries(
      Object.entries(coverage).map(([file, data]) => {
        const absolute = path.join(root, file)
        return [absolute, { ...data, path: absolute }]
      }),
    ),
  )
  const context = libReport.createContext({ dir: coverageDir, coverageMap })
  reports.create('cobertura').execute(context)
  reports.create('text-summary').execute(context)

  const summary = coverageMap.getCoverageSummary()
  const failures = Object.entries(thresholds)
    .filter(([metric, minimum]) => summary[metric].pct < minimum)
    .map(([metric, minimum]) => `${metric} ${summary[metric].pct}% < ${minimum}%`)
  failures.forEach((failure) => console.error(`Coverage below threshold: ${failure}`))
  return failures.length === 0 ? 0 : 1
}

const runAffected = (changed) => {
  const touched = new Set(
    changed.filter((file) => file.startsWith('src/')).flatMap((file) => [file, sourceOf(file)]),
  )
  const existing = [...touched].filter((file) => existsSync(file))
  console.log(`Affected run: ${touched.size} changed file(s) under src/ since ${args.base}`)
  existing.forEach((file) => console.log(`  ${file}`))

  let partial = {}
  if (existing.length > 0) {
    const status = vitest(
      [
        'related',
        '--run',
        '--coverage',
        '--coverage.reporter=json',
        '--passWithNoTests',
        ...vitestReporters,
        ...existing,
      ],
      {
        COVERAGE_PARTIAL: '1',
      },
    )
    if (status !== 0) return status
    partial = readCoverage(vitestOutput)
  }

  const baseline = readCoverage(args.baseline)
  const merged = Object.fromEntries([
    ...Object.entries(baseline).filter(([file]) => !touched.has(file) && existsSync(file)),
    ...Object.entries(partial).filter(([file]) => touched.has(file)),
  ])
  writeBaseline(merged)
  return checkThresholds(merged)
}

const run = () => {
  if (args.full) return runFull('--full')
  if (!args.base || !args.baseline || !existsSync(args.baseline))
    return runFull('no coverage baseline')
  const changed = changedFiles(args.base)
  if (!changed) return runFull(`cannot diff against ${args.base}`)
  const trigger = changed.find((file) => fullRunTriggers.some((pattern) => pattern.test(file)))
  if (trigger) return runFull(`${trigger} changed`)
  return runAffected(changed)
}

process.exit(run())
