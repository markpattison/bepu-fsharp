module BepuTest.Game

open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Game1() as this =
    inherit Game()
    let mutable input = Unchecked.defaultof<Input.State>
    let mutable gameContent = Unchecked.defaultof<Sample.Content>

    let mutable lookAt = Vector3.Zero
    let mutable lookDistance = 20.0f
    let mutable horizontalRotation = float32 (Math.PI * 7.0 / 4.0)
    let mutable verticalRotation = float32 (-Math.PI / 4.0)

    let graphics = new GraphicsDeviceManager(this)

    do graphics.GraphicsProfile <- GraphicsProfile.HiDef

    do graphics.PreferredBackBufferWidth <- 800
    do graphics.PreferredBackBufferHeight <- 600
    do graphics.IsFullScreen <- false

    do graphics.ApplyChanges()
    do base.Content.RootDirectory <- "content"

    let updateInputState() =
        input <- Keyboard.GetState() |> Input.updated input

    override this.Initialize() =
        base.Initialize()

    override this.LoadContent() =
        input <- Input.initialState()

        gameContent <- Sample.loadContent this this.GraphicsDevice

    override this.Update(gameTime) =
        updateInputState()

        if Input.justPressed input Keys.Escape then this.Exit()

        gameContent <- Sample.update gameTime gameContent

        base.Update(gameTime)

    override this.Draw(gameTime) =
        let device = this.GraphicsDevice

        let cameraLocation = Vector3.Transform(Vector3(0.0f, 0.0f, lookDistance), Matrix.CreateRotationX(verticalRotation) * Matrix.CreateRotationY(horizontalRotation))
        let viewMatrix = Matrix.CreateLookAt(cameraLocation, lookAt, Vector3.UnitY)
        let projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.1f, 100.0f)

        Sample.draw device viewMatrix projectionMatrix gameContent gameTime
        
        base.Draw(gameTime)