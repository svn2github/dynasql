using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Perceiveit.Data.Profile;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Perceiveit.Data.UnitTests
{
    public class TestContextProfiler : IDBProfiler
    {
        #region private class TextContextProfileData : IDBProfileExecData

        private class TextContextProfileData : IDBProfileExecData
        {
            private TimeSpan _start;
            private Stopwatch _timer;

            public TextContextProfileData()
            {
                _start = new TimeSpan(DateTime.Now.Ticks);
                _timer = Stopwatch.StartNew();
            }

            public TimeSpan Started
            {
                get { return _start; }
            }

            public TimeSpan Ended
            {
                get { return _timer.Elapsed - _start; }
            }

            public bool HasEnded
            {
                get { return _timer.IsRunning == false; }
            }

            public TimeSpan Duration
            {
                get
                {
                    if (!HasEnded)
                        return TimeSpan.Zero;
                    else
                        return _timer.Elapsed;
                }
            }

            public void EndNow()
            {
                _timer.Stop();
            }
        }

        #endregion

        private string _name;
        private TestContext _context;

        public TestContextProfiler(string name, TestContext context)
        {
            this._name = name;
            this._context = context;
        }


        public string ProfilerName
        {
            get { return _name; }
        }

        public bool CollectsSummaries
        {
            get { return false; }
        }

        public IDBProfileExecData BeginExecution(string connectonName, string sql, params object[] parameters)
        {
            if (null != this._context)
                _context.WriteLine("{0}: {1}", this._name, sql);
            TextContextProfileData data = new TextContextProfileData();
            return data;
        }

        public void EndExecution(IDBProfileExecData data)
        {
            TextContextProfileData tcdata = data as TextContextProfileData;
            tcdata.EndNow();
            if (null != this._context)
            {
                _context.WriteLine("{0}: Completed in {1}\r\n", _name, tcdata.Duration);
            }
        }

        public void DumpExecutionSummary()
        {
            
        }
    }
}
