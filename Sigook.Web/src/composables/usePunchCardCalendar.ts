import { computed, ref, watch, type ComputedRef, type Ref } from 'vue'
import dayjs from 'dayjs'
import { showAlertError } from '@/utils/toast'
import { distributeHours } from '@/utils/distributeHours'
import { maximumHoursPerDay } from '@/constants/catalog'
import { WorkerRequestStatus } from '@/constants/enums'
import type { PunchCardDay, PunchCardWeek, PunchCardWorker } from '@/types/company'

const momentFormat = 'YYYY-MM-DD'

export interface PunchCardCalendarOptions {
  highlights: () => PunchCardDay[] | undefined
  startDate: () => string | Date | undefined
  worker: () => PunchCardWorker | undefined
  onMonthChange: (range: { startDate: string; endDate: string }) => void
}

export interface PunchCardCalendar {
  calendar: Ref<PunchCardWeek[]>
  weekdays: string[]
  monthLabel: ComputedRef<string>
  yearLabel: ComputedRef<string>
  getPreviousMonth: () => void
  getNextMonth: () => void
  getTodayMonth: () => void
  isToday: (date: string | Date) => boolean
  notCurrentMonth: (date: string | Date) => boolean
  isDayEditable: (date: string | Date) => boolean
  dayKey: (date: string | Date) => string
  weekRange: (week: PunchCardWeek) => string
  distributeWeekHours: (week: PunchCardWeek) => void
}

export function usePunchCardCalendar(options: PunchCardCalendarOptions): PunchCardCalendar {
  const calendar = ref<PunchCardWeek[]>([])
  const today = dayjs().toDate()
  const selectDate = ref<string | Date>(today)
  const weekdays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

  const monthLabel = computed(() => dayjs(selectDate.value).format('MMMM'))
  const yearLabel = computed(() => dayjs(selectDate.value).format('YYYY'))

  function getCurrentMonth() {
    calendar.value = []
    const startDay = dayjs(selectDate.value).startOf('month').startOf('week')
    const endDay = dayjs(selectDate.value).endOf('month').endOf('week')
    let date = startDay
    while (date.isBefore(endDay, 'day') || date.isSame(endDay, 'day')) {
      const week: PunchCardWeek = { totalHoursWeek: 0, days: [] }
      for (let i = 0; i < 7; i++) {
        week.days.push({ id: null, day: date.toDate(), totalHoursApproved: 0 })
        date = date.add(1, 'day')
      }
      calendar.value.push(week)
    }
    options.onMonthChange({ startDate: startDay.format(momentFormat), endDate: endDay.format(momentFormat) })
  }

  function getTodayMonth() {
    selectDate.value = dayjs(today).startOf('month').format(momentFormat)
    getCurrentMonth()
  }

  function getNextMonth() {
    selectDate.value = dayjs(selectDate.value).add(1, 'month').toDate()
    getCurrentMonth()
  }

  function getPreviousMonth() {
    selectDate.value = dayjs(selectDate.value).subtract(1, 'month').toDate()
    getCurrentMonth()
  }

  function isToday(date: string | Date) {
    return dayjs(date).format(momentFormat) === dayjs(today).format(momentFormat)
  }

  function notCurrentMonth(date: string | Date) {
    return dayjs(date).format('MMMM') !== dayjs(selectDate.value).format('MMMM')
  }

  function dayKey(date: string | Date) {
    return dayjs(date).format(momentFormat)
  }

  function isAvailableToUpdate(date: string | Date) {
    const start = dayjs(options.startDate()).subtract(1, 'day')
    const oneMonth = dayjs().add(1, 'month')
    return dayjs(date).toDate() > start.toDate() && dayjs(date).toDate() < oneMonth.toDate()
  }

  function isAvailableToUpdateWorker(date: string | Date) {
    const worker = options.worker()
    if (worker && worker.workerRequestStatus === WorkerRequestStatus.Rejected && worker.rejectedAt) {
      const start = dayjs(options.startDate()).subtract(1, 'day')
      const oneMonth = dayjs(worker.rejectedAt).add(1, 'month')
      return dayjs(date).toDate() > start.toDate() && dayjs(date).toDate() < oneMonth.toDate()
    }
    return true
  }

  function isDayEditable(date: string | Date) {
    return isAvailableToUpdate(date) && isAvailableToUpdateWorker(date)
  }

  function weekRange(week: PunchCardWeek) {
    const first = week.days[0]
    const last = week.days[week.days.length - 1]
    if (!first || !last) return ''
    return `${dayjs(first.day).format('MMM D')} – ${dayjs(last.day).format('MMM D')}`
  }

  function distributeWeekHours(week: PunchCardWeek) {
    const hours = distributeHours(week.days.length, week.totalHoursWeek ?? 0, maximumHoursPerDay)
    if (hours.length > 0) {
      for (let i = 0; i < hours.length; i++) {
        week.days[i].totalHoursApproved = hours[i] || 0
      }
    } else {
      showAlertError('Total hours is invalid')
    }
  }

  function syncHighlightsWithCalendar() {
    const highlights = options.highlights()
    if (!highlights || !calendar.value.length) return
    for (const week of calendar.value) {
      for (let iDay = 0; iDay < week.days.length; iDay++) {
        const currentDay = dayKey(week.days[iDay].day)
        const match = highlights.find((d) => dayKey(d.day) === currentDay)
        if (match) {
          week.days[iDay] = match
        } else if (week.days[iDay].id) {
          week.days[iDay] = { id: null, day: week.days[iDay].day, totalHoursApproved: 0 }
        }
      }
      week.totalHoursWeek = week.days.reduce(
        (acc, day) => acc + (day.totalHoursApproved ?? 0),
        0,
      )
    }
  }

  watch(() => options.highlights(), syncHighlightsWithCalendar, { immediate: true })

  getTodayMonth()

  return {
    calendar,
    weekdays,
    monthLabel,
    yearLabel,
    getPreviousMonth,
    getNextMonth,
    getTodayMonth,
    isToday,
    notCurrentMonth,
    isDayEditable,
    dayKey,
    weekRange,
    distributeWeekHours,
  }
}
