module BepuTest.Sample

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Content =
    {
        Effect: Effect
        SphereVertices: VertexPositionNormalColour []
        SphereIndices: int []
        CubeVertices: VertexPositionNormalColour []
        CubeIndices: int []
        Simulation: BepuPhysics.Simulation
        SphereRef: BepuPhysics.BodyReference
        BoxRef: BepuPhysics.StaticReference
        ThreadDispatcher: BepuUtilities.IThreadDispatcher
    }

let loadContent (this: Game) device =

    let sphereVertices, sphereIndices =
        Sphere.create 2
        |> Sphere.getVerticesAndIndices Sphere.Smooth Sphere.OutwardFacing

    let simulation, sphereRef, boxRef = SimpleSelfContainedDemo.createSimulation()

    let threadDispatcher = new SimpleThreadDispatcher(System.Environment.ProcessorCount)

    {
        Effect = this.Content.Load<Effect>("effects")
        SphereVertices = sphereVertices
        SphereIndices = sphereIndices
        CubeVertices = Cube.vertices Color.Green
        CubeIndices = Cube.indices

        Simulation = simulation
        SphereRef = sphereRef
        BoxRef = boxRef
        ThreadDispatcher = threadDispatcher
    }

let update (gameTime: GameTime) content =
    let dt = single gameTime.ElapsedGameTime.TotalSeconds

    if dt > 0.0f then
        content.Simulation.Timestep(dt, content.ThreadDispatcher)

    content

let draw (device: GraphicsDevice) (viewMatrix: Matrix) (projectionMatrix: Matrix) content (gameTime: GameTime) =
    let time = (single gameTime.TotalGameTime.TotalMilliseconds) / 100.0f

    do device.Clear(Color.LightGray)

    let effect = content.Effect

    effect.CurrentTechnique <- effect.Techniques.["Cube"]
    effect.Parameters.["xView"].SetValue(viewMatrix)
    effect.Parameters.["xProjection"].SetValue(projectionMatrix)

    // sphere

    let spherePos = content.SphereRef.Pose.Position

    effect.Parameters.["xWorld"].SetValue(Matrix.CreateTranslation(spherePos.X, spherePos.Y, spherePos.Z))
    effect.Parameters.["xAmbient"].SetValue(0.2f)
    effect.Parameters.["xLightPosition"].SetValue(Vector3(-10.0f, 5.0f, 0.0f))

    effect.CurrentTechnique.Passes |> Seq.iter
        (fun pass ->
            pass.Apply()
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, content.SphereVertices, 0, content.SphereVertices.Length, content.SphereIndices, 0, content.SphereIndices.Length / 3)
        )
    
    // box

    let boxPos = content.BoxRef.Pose.Position
    let boxRot = content.BoxRef.BoundingBox // TODO

    effect.Parameters.["xWorld"].SetValue(Matrix.CreateTranslation(boxPos.X, boxPos.Y, boxPos.Z) * Matrix.CreateScale(2.5f, 1.0f, 2.5f))
    effect.Parameters.["xAmbient"].SetValue(0.2f)
    effect.Parameters.["xLightPosition"].SetValue(Vector3(-10.0f, 5.0f, 0.0f))

    effect.CurrentTechnique.Passes |> Seq.iter
        (fun pass ->
            pass.Apply()
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, content.CubeVertices, 0, content.CubeVertices.Length, content.CubeIndices, 0, content.CubeIndices.Length / 3)
        )
