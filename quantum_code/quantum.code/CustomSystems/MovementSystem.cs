using Photon.Deterministic;

namespace Quantum
{
    public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>
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
            var input = f.GetPlayerInput(filter.Link->PlayerRef);

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
    }
}