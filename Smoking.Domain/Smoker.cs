using System;
using System.Collections.Generic;

namespace Smoking.Domain
{
    public class Smoker
    {
        public Guid AggregateID { get; private set; }
        public DateTimeOffset? LastSmoked { get; private set; }
        public int LimitPerDay { get; private set; }
        public int TodayConsumed { get; private set; }
        public TimeSpan Interval { get; private set; }

        public List<Event> Changes { get; set; } = new List<Event>();

        public Smoker(int limitPerDay, TimeSpan interval)
        {
            this.AggregateID = Guid.NewGuid();
            this.LastSmoked = null;
            this.LimitPerDay = limitPerDay;
            this.TodayConsumed = 0;
            this.Interval = interval;

            this.Changes.Add(new SmokerRegistered
            {
                AggregateID = AggregateID,
                OccurredOn = DateTimeOffset.Now,
                LimitPerDay = this.LimitPerDay,
                Interval = this.Interval,
            });
        }

        private Smoker()
        {
        }

        public static Smoker Replay(List<Event> events)
        {
            var smoker = new Smoker();

            foreach (Event @event in events)
            {
                switch (@event)
                {
                    case SmokerRegistered registered:
                        smoker.AggregateID = registered.AggregateID;
                        smoker.LastSmoked = null;
                        smoker.LimitPerDay = registered.LimitPerDay;
                        smoker.TodayConsumed = 0;
                        smoker.Interval = registered.Interval;
                        break;

                    case SmokerSmoked smoked:
                        smoker.TodayConsumed++;
                        smoker.LastSmoked = smoked.OccurredOn;
                        break;

                    case SmokerLimitExceeded:
                        break;
                }
            }
            return smoker;
        }

        public void Smoke(DateTimeOffset now)
        {
            var fromLastSmoked = now - (this.LastSmoked ?? DateTimeOffset.MinValue);
            // TODO raise SmokeIntervalViolated
            var lastday = this.LastSmoked?.Day;
            if (lastday == null || lastday != now.Day)
            {
                // Next day
                this.TodayConsumed = 0;
            }

            this.TodayConsumed++;
            this.LastSmoked = now;


            this.Changes.Add(new SmokerSmoked
            {
                AggregateID = this.AggregateID,
                OccurredOn = now,
            });

            if (this.TodayConsumed > this.LimitPerDay)
            {
                this.Changes.Add(new SmokerLimitExceeded
                {
                    AggregateID = this.AggregateID,
                    OccurredOn = now,
                });
            }
        }
    }
}
