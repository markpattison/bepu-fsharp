module BepuTest.Program

[<EntryPoint>]
let main argv =
    // SimpleSelfContainedDemo.Run()
    let game = new Game.Game1()
    do game.Run()    
    0