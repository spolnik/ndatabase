using System;
using System.Threading;
using NDatabase.Odb;
using NDatabase.Odb.Core;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Tool.Mutex
{
    /// <summary>
    ///   A Simple Mutex for lock operations
    /// </summary>
    /// <author>osmadja</author>
    public sealed class Mutex
    {
        /// <summary>
        ///   The name of the mutex
        /// </summary>
        private readonly string _name;

        private bool _debug;

        /// <summary>
        ///   The lock status *
        /// </summary>
        private bool _inUse;

        private int _nbOwners;

        public Mutex(string name)
        {
            _name = name;
            _inUse = false;
            _debug = false;
            _nbOwners = 0;
        }

        /// <exception cref="System.Exception"></exception>
        public Mutex Acquire(string who)
        {
            if (_debug)
            {
                var message = string.Format("Thread {0} - {1} : Trying to acquire mutex {2}",
                                            OdbThreadUtil.GetCurrentThreadName(), who, _name);
                DLogger.Info(message);
            }

            //DLogger.info("From " + StringUtils.exceptionToString(new Exception(), false));
            
            lock (this)
            {
                while (_inUse)
                {
                    //TODO: check it
                    Monitor.Wait(this);
                }
                if (_nbOwners != 0)
                    throw new Exception("nb owners != 0 - " + _nbOwners);
                _inUse = true;
                _nbOwners++;
            }
            if (_debug)
                DLogger.Info("Thread " + OdbThreadUtil.GetCurrentThreadName() + " - " + who + " : Mutex " + _name +
                             " acquired!");

            return this;
        }

        public void Release(string who)
        {
            lock (this)
            {
                if (_debug)
                    DLogger.Info("Thread " + OdbThreadUtil.GetCurrentThreadName() + " - " + who + " : Releasing mutex " +
                                 _name);
                _inUse = false;
                _nbOwners--;
                if (_nbOwners < 0)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter("Nb owner is negative in release(" + who + ")"));
                }
            }
        }

        /// <exception cref="System.Exception"></exception>
        public bool Attempt(long msecs)
        {
            lock (this)
            {
                if (!_inUse)
                {
                    _inUse = true;
                    _nbOwners++;
                    return true;
                }
                if (msecs <= 0)
                    return false;
                
                var waitTime = msecs;
                var start = OdbTime.GetCurrentTimeInMs();
                while (true)
                {
                    //TODO: check it
                    Monitor.Wait(this, (int) waitTime);

                    if (!_inUse)
                    {
                        _inUse = true;
                        _nbOwners++;
                        return true;
                    }
                    waitTime = msecs - (OdbTime.GetCurrentTimeInMs() - start);
                    if (waitTime <= 0)
                        return false;
                }
            }
        }

        public string GetName()
        {
            return _name;
        }

        public void SetDebug(bool debug)
        {
            _debug = debug;
        }

        public bool IsInUse()
        {
            return _inUse;
        }

        public int GetNbOwners()
        {
            return _nbOwners;
        }
    }
}
