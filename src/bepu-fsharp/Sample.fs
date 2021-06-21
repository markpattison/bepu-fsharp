module BepuTest.Sample

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Content =
    {
        Effect: Effect
        SphereVertices: VertexPositionNormalColour []
        SphereIndices: int []
        Simulation: BepuPhysics.Simulation
        SphereRef: BepuPhysics.BodyReference
    }

let loadContent (this: Game) device =

    let sphereVertices, sphereIndices =
        Sphere.create 2
        |> Sphere.getVerticesAndIndices Sphere.Smooth Sphere.OutwardFacing

    let simulation, sphereRef = SimpleSelfContainedDemo.createSimulation()

    {
        Effect = this.Content.Load<Effect>("effects")
        SphereVertices = sphereVertices
        SphereIndices = sphereIndices

        Simulation = simulation
        SphereRef = sphereRef
    }

let update (gameTime: GameTime) content =
    let dt = single gameTime.ElapsedGameTime.TotalSeconds

    if dt > 0.0f then
        content.Simulation.Timestep(dt)

    content

let draw (device: GraphicsDevice) (viewMatrix: Matrix) (projectionMatrix: Matrix) content (gameTime: GameTime) =
    let time = (single gameTime.TotalGameTime.TotalMilliseconds) / 100.0f
    let effect = content.Effect

    do device.Clear(Color.LightGray)

    let spherePos = content.SphereRef.Pose.Position

    effect.CurrentTechnique <- effect.Techniques.["Cube"]
    effect.Parameters.["xView"].SetValue(viewMatrix)
    effect.Parameters.["xProjection"].SetValue(projectionMatrix)
    effect.Parameters.["xWorld"].SetValue(Matrix.CreateTranslation(spherePos.X, spherePos.Y, spherePos.Z))
    effect.Parameters.["xAmbient"].SetValue(0.2f)
    effect.Parameters.["xLightPosition"].SetValue(Vector3(-10.0f, 5.0f, 0.0f))

    effect.CurrentTechnique.Passes |> Seq.iter
        (fun pass ->
            pass.Apply()
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, content.SphereVertices, 0, content.SphereVertices.Length, content.SphereIndices, 0, content.SphereIndices.Length / 3)
        )
