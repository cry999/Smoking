using System;
using System.IO;
using System.Threading.Tasks;
using Smoking.Domain;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Smoking.Infra
{
    public class SQLiteSmokingRepository : SmokerRepository
    {
        public SQLiteSmokingRepository(string settingDirectory)
        {
            this.SettingDirectory = settingDirectory;
        }

        private const string DataSource = "smoking.db";

        private string SettingDirectory { get; set; }

        private string AbsoluteDataSource { get => Path.Join(SettingDirectory, DataSource); }

        private string SQLiteDataSource { get => $"Data Source={this.AbsoluteDataSource}"; }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(this.SQLiteDataSource);
        }

        public async Task Migrate()
        {
            using (var connection = this.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    CREATE TABLE smoking_events (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        aggregate_id TEXT,
                        occurred_on TEXT,
                        event_type TEXT,
                        payload TEXT
                    )
                    ";
                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                CREATE INDEX idx_smoking_events_aggregate_id ON smoking_events (aggregate_id)
                ";
                }
            }
        }

        public async Task<Smoker> Get(Guid aggregateID)
        {
            var events = new List<Event>();
            using (var connection = this.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {

                    command.CommandText = @"
                    SELECT
                        occurred_on,
                        event_type,
                        payload
                    FROM
                        smoking_events
                    WHERE
                        aggregate_id = $aggregate_id
                    ";
                    command.Parameters.AddWithValue("$aggregate_id", aggregateID);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var occurredOn = DateTimeOffset.Parse(reader.GetString(0));
                            var eventType = reader.GetString(1);
                            var payload = reader.GetString(2);
                            switch (eventType)
                            {
                                case SmokerRegistered.EventType:
                                    var registered = JsonConvert.DeserializeObject<SmokerRegistered>(payload);
                                    events.Add(registered);
                                    break;
                                case SmokerLimitExceeded.EventType:
                                    var exceeded = JsonConvert.DeserializeObject<SmokerLimitExceeded>(payload);
                                    events.Add(exceeded);
                                    break;
                                case SmokerSmoked.EventType:
                                    var smoked = JsonConvert.DeserializeObject<SmokerSmoked>(payload);
                                    events.Add(smoked);
                                    break;
                            }
                        }
                    }
                }
            }
            if (events.Count < 1)
            {
                throw new Exception("Not Found");
            }
            return Smoker.Replay(events);
        }

        public async Task Put(Smoker smoker)
        {
            using (var connection = this.GetConnection())
            {
                await connection.OpenAsync();

                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        foreach (var @event in smoker.Changes)
                        {
                            command.CommandText = @"
                                INSERT INTO smoking_events (
                                    aggregate_id,
                                    occurred_on,
                                    event_type,
                                    payload
                                ) VALUES (
                                    $aggregate_id,
                                    $occurred_on,
                                    $event_type,
                                    $payload
                                )
                            ";
                            command.Parameters.AddWithValue("$aggregate_id", @event.AggregateID);
                            command.Parameters.AddWithValue("$occurred_on", @event.OccurredOn);
                            command.Parameters.AddWithValue("$event_type", @event.Type);
                            command.Parameters.AddWithValue("$payload", JsonConvert.SerializeObject(@event));
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch
                {
                    await transaction.RollbackAsync();
                }
                finally
                {
                    await transaction.CommitAsync();
                }
            }
        }
    }
}
