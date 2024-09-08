using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Redapple
{
	public class GameEvent : UnityEvent<object, object[]> { }
	public class OnEventFired : UnityEvent<object, EventID, object[]> { }

	public class EventManager : Singleton<EventManager>, IManager
	{
		/// <summary>
		/// Set to true to log all fired events to console.
		/// </summary>
		private static readonly bool _DEBUG_LOG_EVENTS = false;

		public OnEventFired OnEventFiredEvent = new OnEventFired();

		private readonly Dictionary<EventID, GameEvent> EventList = new Dictionary<EventID, GameEvent>();
		private readonly List<EventID> EventFrame = new List<EventID>();

		public IEnumerator Init() { yield break; }
		public IEnumerator OnLogin() { yield break; }

		/// <summary>
		/// Register a function to listen to all EventIDs 
		/// </summary>
		/// <param name="_ManagerEvent"></param>
		public static void RegisterAllEvents(UnityAction<object, EventID, object[]> _ManagerEvent)
		{
			if (InstanceValid)
			{ 
				if (Instance.EventList != null)
                    Instance.OnEventFiredEvent.AddListener(_ManagerEvent);
			}
		}

		/// <summary>
		/// De-registers a listener for all events.
		/// </summary>
		public static void DeregisterAllEvents(UnityAction<object, EventID, object[]> _ManagerEvent)
		{
			if (InstanceValid)
			{
				if (Instance.EventList != null)
					Instance.OnEventFiredEvent.RemoveListener(_ManagerEvent);
			}
		}

		private void Update()
		{
			// Reset List After The Frame 
			EventFrame.Clear();
		}

		/// <summary>
		/// Register a function to listen to a specific EventID 
		/// </summary>
		/// <param name="_EventID"></param>
		/// <param name="_Event"></param>
		public static void RegisterEvent(EventID _EventID, UnityAction<object, object[]> _Event)
		{
			if (InstanceValid)
			{
				if (Instance.EventList != null)
				{
					if(!Instance.EventList.ContainsKey(_EventID))
					{
						GameEvent newGameEvent = new GameEvent();
						Instance.EventList.Add(_EventID, newGameEvent);
					}

					Instance.EventList[_EventID].RemoveListener(_Event);
					Instance.EventList[_EventID].AddListener(_Event);
				}
			}
		}

		/// <summary>
		/// Register a function to listen to a specific EventID 
		/// </summary>
		/// <param name="_EventID"></param>
		/// <param name="_Event"></param>
		public static void DeregisterEvent(EventID _EventID, UnityAction<object, object[]> _Event)
		{
			if (InstanceValid)
			{
				if (Instance.EventList != null)
				{
					if (Instance.EventList.ContainsKey(_EventID))
						Instance.EventList[_EventID].RemoveListener(_Event);
				}
			}
		}

		/// <summary>
		/// Handles firing events from the event manager
		/// </summary>
		/// <param name="_Sender"></param>
		/// <param name="_EventID"></param>
		/// <param name="_EventData"></param>
		public static void FireEvent(System.Object _Sender, EventID _EventID, params object[] _EventData)
		{
			if (InstanceValid && _EventID != EventID.NONE)
			{
				if (Application.isEditor && _DEBUG_LOG_EVENTS)
					Debug.Log($"Event Fired: {_EventID}, Sender: {_Sender}");

				if (Instance.EventList != null)
				{
					///------------------------------------------------------------------------
					//                      	EVENT FUNCTION
					///------------------------------------------------------------------------
					if (Instance.EventList.ContainsKey(_EventID))
						Instance.EventList[_EventID].Invoke(_Sender, _EventData);

					///------------------------------------------------------------------------
					//                      	EVENT FRAME
					///------------------------------------------------------------------------
					if (!Instance.EventFrame.Contains(_EventID))
						Instance.EventFrame.Add(_EventID);
					Instance.OnEventFiredEvent.Invoke(_Sender, _EventID, _EventData);
				}
			}
		}

		public static bool IsEventFired(EventID _EventID)
		{
			return InstanceValid ? Instance.EventFrame.Contains(_EventID) : false;
		}
	}
}
