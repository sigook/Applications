import 'dart:convert';
import 'package:sqflite/sqflite.dart';
import 'package:path/path.dart';
import '../models/job_model.dart';

abstract class JobsLocalDataSource {
  Future<List<JobModel>> getCachedJobs();
  Future<void> cacheJobs(List<JobModel> jobs);
  Future<void> clearCache();
  Future<DateTime?> getLastCacheTime();
}

class JobsLocalDataSourceImpl implements JobsLocalDataSource {
  static const String _tableName = 'jobs';
  static const String _cacheTimeKey = 'cache_metadata';
  Database? _database;

  Future<Database> get database async {
    if (_database != null) return _database!;
    _database = await _initDatabase();
    return _database!;
  }

  Future<Database> _initDatabase() async {
    final databasesPath = await getDatabasesPath();
    final path = join(databasesPath, 'sigook_jobs.db');

    return await openDatabase(
      path,
      version: 1,
      onCreate: (db, version) async {
        await db.execute('''
          CREATE TABLE $_tableName (
            id TEXT PRIMARY KEY,
            data TEXT NOT NULL,
            cached_at INTEGER NOT NULL
          )
        ''');

        await db.execute('''
          CREATE TABLE $_cacheTimeKey (
            key TEXT PRIMARY KEY,
            timestamp INTEGER NOT NULL
          )
        ''');
      },
    );
  }

  @override
  Future<List<JobModel>> getCachedJobs() async {
    final db = await database;
    final List<Map<String, dynamic>> maps = await db.query(
      _tableName,
      orderBy: 'cached_at DESC',
    );

    return maps.map((map) {
      final jobData = json.decode(map['data'] as String);
      return JobModel.fromJson(jobData);
    }).toList();
  }

  @override
  Future<void> cacheJobs(List<JobModel> jobs) async {
    final db = await database;
    final batch = db.batch();

    // Clear old data
    batch.delete(_tableName);

    // Insert new jobs
    final now = DateTime.now().millisecondsSinceEpoch;
    for (final job in jobs) {
      batch.insert(_tableName, {
        'id': job.id,
        'data': json.encode(job.toJson()),
        'cached_at': now,
      }, conflictAlgorithm: ConflictAlgorithm.replace);
    }

    // Update cache time
    batch.insert(_cacheTimeKey, {
      'key': 'last_cache_time',
      'timestamp': now,
    }, conflictAlgorithm: ConflictAlgorithm.replace);

    await batch.commit(noResult: true);
  }

  @override
  Future<void> clearCache() async {
    final db = await database;
    await db.delete(_tableName);
    await db.delete(_cacheTimeKey);
  }

  @override
  Future<DateTime?> getLastCacheTime() async {
    final db = await database;
    final List<Map<String, dynamic>> maps = await db.query(
      _cacheTimeKey,
      where: 'key = ?',
      whereArgs: ['last_cache_time'],
    );

    if (maps.isEmpty) return null;

    final timestamp = maps.first['timestamp'] as int;
    return DateTime.fromMillisecondsSinceEpoch(timestamp);
  }
}
