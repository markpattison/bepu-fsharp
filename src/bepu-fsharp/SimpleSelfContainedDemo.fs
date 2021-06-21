module BepuTest.SimpleSelfContainedDemo

open BepuPhysics
open BepuPhysics.Collidables
open BepuPhysics.CollisionDetection
open BepuPhysics.Constraints
open BepuUtilities
open BepuUtilities.Memory
open System
open System.Collections.Generic
open System.Numerics
open System.Runtime.CompilerServices
open System.Text

[<Struct>]
type NarrowPhaseCallbacks =
    interface INarrowPhaseCallbacks with

        member this.Initialize _ = ()

        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.AllowContactGeneration(_, a, b) =
            a.Mobility = CollidableMobility.Dynamic || b.Mobility = CollidableMobility.Dynamic
        
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.AllowContactGeneration(_, pair, _, _) =
            true
        
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.ConfigureContactManifold<'TManifold when 'TManifold : struct and 'TManifold :> IContactManifold<'TManifold>>(_, pair, manifold: byref<'TManifold>, pairMaterial: byref<PairMaterialProperties>) =
            pairMaterial.FrictionCoefficient <- 1.0f
            pairMaterial.MaximumRecoveryVelocity <- Single.MaxValue
            pairMaterial.SpringSettings <- SpringSettings(5.0f, 0.01f)
            true
        
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.ConfigureContactManifold(_, pair, _, _, manifold: byref<ConvexContactManifold>) =
            true
        
        member this.Dispose() = ()

    interface IDisposable with
        member this.Dispose() = ()

[<Struct>]
type PoseIntegratorCallbacks (gravity: Vector3) =
    [<DefaultValue>]
    val mutable gravityDt: Vector3
    
    interface IPoseIntegratorCallbacks with

        member this.Initialize _ = ()

        member this.AngularIntegrationMode = AngularIntegrationMode.Nonconserving

        member this.PrepareForIntegration (dt: single) =
            this.gravityDt <- gravity * dt
        
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.IntegrateVelocity(bodyIndex, pose: inref<RigidPose>, localInertia: inref<BodyInertia>, _, velocity: byref<BodyVelocity>) =
            if localInertia.InverseMass > 0.0f then
                velocity.Linear <- velocity.Linear + this.gravityDt
            else
                ()

let createSimulation() =
    let bufferPool = new BufferPool()

    let simulation =
        Simulation.Create(
            bufferPool,
            new NarrowPhaseCallbacks(),
            PoseIntegratorCallbacks(Vector3(0.0f, -10.0f, 0.0f)),
            PositionLastTimestepper())

    let sphere = Sphere(1.0f)
    let spherePosition = Vector3(-2.6f, 10.0f, 0.0f)
    let mutable sphereInertia = BodyInertia()
    let sphereCollidable = CollidableDescription(simulation.Shapes.Add(&sphere), 0.1f)
    let sphereActivity = BodyActivityDescription(0.00f)
    sphere.ComputeInertia(1.0f, &sphereInertia)
    let sphereDescription = BodyDescription.CreateDynamic(&spherePosition, &sphereInertia, &sphereCollidable, &sphereActivity)

    let box = Box(5.0f, 1.0f, 5.0f)
    let boxPosition = Vector3.Zero
    let boxCollidable = CollidableDescription(simulation.Shapes.Add(&box), 0.1f)
    let boxDescription = StaticDescription(&boxPosition, &boxCollidable)

    let sphereRef = simulation.Bodies.GetBodyReference(simulation.Bodies.Add(&sphereDescription))
    let boxRef = simulation.Statics.GetStaticReference(simulation.Statics.Add(&boxDescription))

    simulation, sphereRef, boxRef

let Run() =
    let simulation, sphereRef, boxRef = createSimulation()

    for i in 0 .. 199 do
        simulation.Timestep(0.01f)
        printfn "%0.3f" sphereRef.Pose.Position.Y

    ()
