namespace Quantum
{
    public unsafe class PlayerRespawnSystem : SystemSignalsOnly,
        ISignalOnTrigger3D,
        ISignalOnTriggerExit3D
    {
        public void OnTrigger3D(Frame f, TriggerInfo3D info)
        {
            if (!IsRelevant(f, info.Entity, info.Other)) return;
            ResetPlayerPosition(f, info.Other);
        }

        public void OnTriggerExit3D(Frame f, ExitInfo3D info)
        {
        }

        private static bool IsRelevant(Frame f, EntityRef entity, EntityRef other)
        {
            return f.Has<Respawn>(entity) && f.Has<PlayerLink>(other);
        }

        private static void ResetPlayerPosition(Frame f, EntityRef playerEntityRef)
        {
            if (f.Unsafe.TryGetPointer<Transform3D>(playerEntityRef, out var t))
            {
                t->Position.X = 0;
                t->Position.Z = 0;
                t->Position.Y = 2;
            }
        }
    }
}