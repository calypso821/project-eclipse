namespace Eclipse.Engine.Core
{
    internal enum SystemGroup
    {
        PreUpdate,      // Input, Controllers
        PhysicsUpdate,  // Phycsics (detection, response)
        PostUpdate      // Animation, Render
    }
}
