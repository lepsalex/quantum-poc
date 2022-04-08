using Photon.Deterministic;

namespace Quantum
{
    public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>, ISignalOnPlayerDataSet
    {
        public struct Filter
        {
            public EntityRef Entity;
            public CharacterController3D* KCC;
            public Transform3D* Transform;
            public PlayerLink* Link;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            var input = f.GetPlayerInput(filter.Link->Player);

            if (input->Jump.WasPressed)
            {
                filter.KCC->Jump(f);
            }

            // speed hack protection
            if (input->Direction.SqrMagnitude > 1)
            {
                input->Direction = input->Direction.Normalized;
            }

            filter.KCC->Move(f, filter.Entity, input->Direction.XOY);

            if (input->Direction != default)
            {
                filter.Transform->Rotation = FPQuaternion.LookRotation(input->Direction.XOY);
            }
        }

        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            var data = f.GetPlayerData(player);
            var prototype = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);
            var entityRef = f.Create(prototype);

            // link player
            if (f.Unsafe.TryGetPointer<PlayerLink>(entityRef, out var pl))
            {
                pl->Player = player;
            }

            // update X position
            if (f.Unsafe.TryGetPointer<Transform3D>(entityRef, out var t))
            {
                t->Position.X = 0 + player;
            }
        }
        
        
    }
}