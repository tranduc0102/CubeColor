using System;
using System.Collections.Generic;
using UnityEngine;

public class Observer : Sington<Observer>
{
    private Dictionary<EventID, Action<object>> gameEventsManager = new Dictionary<EventID, Action<object>>();
     
     public void RegisterListener(EventID type, Action<object> callBackAction)
     {
         if (gameEventsManager.ContainsKey(type))
         {
             gameEventsManager[type] += callBackAction;
         }
         else
         {
             gameEventsManager.Add(type, null);
             gameEventsManager[type] += callBackAction;
         }
     }
     
     public void PostEvent(EventID eventID, object param = null)
     {
         if (!gameEventsManager.ContainsKey(eventID))
         {
             Debug.Log("Event has no Listener");
             return;
         }
    
         var callbacks = gameEventsManager[eventID];
         if (callbacks != null)
         {
             callbacks(param);
         }
         else
         {
             Debug.Log("PostEvent " + eventID + "but no listener remain, Remove this key");
             gameEventsManager.Remove(eventID);
         }
     }
     public void RemoveListener(EventID eventID, Action<object> callBackAction)
     {
         if (gameEventsManager.ContainsKey(eventID))
         {
             gameEventsManager[eventID] -= callBackAction;
         }
         else
         {
             Debug.Log("Not Found EventID with id: " + eventID);
         }
     }
     public void RemoveAllListeners()
     {
         gameEventsManager.Clear();
     }
}

