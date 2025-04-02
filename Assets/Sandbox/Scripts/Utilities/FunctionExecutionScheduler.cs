using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// this function is a helper for scheduling functions so only one of each type runs at a time
    /// </summary>
    public class FunctionExecutionScheduler
    {
        /// <summary>
        /// manages the calls for a function
        /// </summary>
        public class FunctionCallTracker
        {
            private Queue<Action<Action>> callQueue = new Queue<Action<Action>>();

            public void JoinQueue(Action<Action> _functionCall)
            {
                callQueue.Enqueue(_functionCall);

                if(callQueue.Count == 1)
                {
                    //indicate to next item in queue its their turn 
                    _functionCall?.Invoke(ExecutionFinished);
                }
            }

            //is there a function currently executing of this type
            public bool IsExecuting()
            {
                return callQueue.Count > 0;
            }

            public void ExecutionFinished()
            {
                callQueue.Dequeue();

                if (callQueue.Count > 0)
                {
                    Action<Action> nextQueueTicket = callQueue.Peek();

                    //indicate to next item in queue its their turn 
                    nextQueueTicket?.Invoke(ExecutionFinished);
                }
            }
        }

        private Dictionary<string, FunctionCallTracker> ActiveFunctionCalls = new Dictionary<string, FunctionCallTracker>();

        /// <summary>
        /// joins a queue for a funciton, when invoked it reuturns a ticket that must be completed so following items can be executed 
        /// </summary>
        /// <param name="_functionName"></param>
        /// <param name="_functionCall"></param>
        public void QueueForFunction(string _functionName, Action<Action> _functionCall)
        {
            //check if there is a function by that name 
            if(ActiveFunctionCalls.TryGetValue(_functionName,out FunctionCallTracker callTracker))
            {
                callTracker.JoinQueue(_functionCall);
            }
            else
            {
                callTracker = new FunctionCallTracker();

                ActiveFunctionCalls.Add(_functionName, callTracker);

                callTracker.JoinQueue(_functionCall);
            }
        }

        /// <summary>
        /// returns true if a registered async function with this name is still completing 
        /// </summary>
        /// <param name="_functionName"></param>
        /// <returns></returns>
        public bool IsExecuting(string _functionName)
        {
            //check if there is a function by that name 
            if (ActiveFunctionCalls.TryGetValue(_functionName, out FunctionCallTracker callTracker))
            {
                return callTracker.IsExecuting();
            }

            return false;
        }

        //generic functions for the above function calls. if your method signature does not match add it to the list

        #region GenericFunctionVariants

        public void QueueForFunction(Action _function, Action<Action> _functionCall)
        {
            string funcName = _function.Method.Name;

            QueueForFunction(funcName, _functionCall);
        }

        public void QueueForFunction<A1>(Action<A1> _function, Action<Action> _functionCall)
        {
            string funcName = _function.Method.Name;

            QueueForFunction(funcName, _functionCall);
        }

        public void QueueForFunction<A1, A2>(Action<A1,A2> _function, Action<Action> _functionCall)
        {
            string funcName = _function.Method.Name;

            QueueForFunction(funcName, _functionCall);
        }

        public void QueueForFunction<A1,A2,A3>(Action<A1,A2,A3> _function, Action<Action> _functionCall)
        {
            string funcName = _function.Method.Name;

            QueueForFunction(funcName, _functionCall);
        }

        public void QueueForFunction<R1>(Func<R1> _function, Action<Action> _functionCall)
        {
            string funcName = _function.Method.Name;

            QueueForFunction(funcName, _functionCall);
        }

        public void QueueForFunction<R1,A1>(Func<R1,A1> _function, Action<Action> _functionCall)
        {
            string funcName = _function.Method.Name;

            QueueForFunction(funcName, _functionCall);
        }

        public void QueueForFunction<R1,A1,A2>(Func<R1,A1,A2> _function, Action<Action> _functionCall)
        {
            string funcName = _function.Method.Name;

            QueueForFunction(funcName, _functionCall);
        }

        public void QueueForFunction<R1,A1,A2,A3>(Func<R1,A1,A2,A3> _function, Action<Action> _functionCall)
        {
            string funcName = _function.Method.Name;

            QueueForFunction(funcName, _functionCall);
        }

        public bool IsExecuting(Action _function)
        {
            string funcName = _function.Method.Name;

            return IsExecuting(funcName);
        }

        public bool IsExecuting<A1>(Action<A1> _function)
        {
            string funcName = _function.Method.Name;

            return IsExecuting(funcName);
        }

        public bool IsExecuting<A1, A2>(Action<A1, A2> _function)
        {
            string funcName = _function.Method.Name;

            return IsExecuting(funcName);
        }

        public bool IsExecuting<A1, A2, A3>(Action<A1, A2, A3> _function)
        {
            string funcName = _function.Method.Name;

            return IsExecuting(funcName);
        }

        public bool IsExecuting<R1>(Func<R1> _function)
        {
            string funcName = _function.Method.Name;

            return IsExecuting(funcName);
        }

        public bool IsExecuting<R1, A1>(Func<R1, A1> _function)
        {
            string funcName = _function.Method.Name;

            return IsExecuting(funcName);
        }

        public bool IsExecuting<R1, A1, A2>(Func<R1, A1, A2> _function)
        {
            string funcName = _function.Method.Name;

            return IsExecuting(funcName);
        }

        public bool IsExecuting<R1, A1, A2, A3>(Func<R1, A1, A2, A3> _function)
        {
            string funcName = _function.Method.Name;

            return IsExecuting(funcName);
        }

        #endregion
    }
}
