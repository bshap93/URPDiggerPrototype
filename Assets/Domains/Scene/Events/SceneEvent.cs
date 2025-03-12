using MoreMountains.Tools;

namespace Domains.Scene.Events
{
    public enum SceneEventType
    {
        SceneLoaded
    }

    public struct SceneEvent
    {
        static SceneEvent _e;

        public SceneEventType EventType;


        public static void Trigger(SceneEventType sceneEventType)
        {
            _e.EventType = sceneEventType;
            MMEventManager.TriggerEvent(_e);
        }
    }
}
