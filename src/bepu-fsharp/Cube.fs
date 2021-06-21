module BepuTest.Cube

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let positions = [|
    // Front face
    (-1.0f, -1.0f,  1.0f)
    (-1.0f,  1.0f,  1.0f)
    ( 1.0f, -1.0f,  1.0f)
    ( 1.0f,  1.0f,  1.0f)

    // Back face
    ( 1.0f, -1.0f, -1.0f)
    ( 1.0f,  1.0f, -1.0f)
    (-1.0f, -1.0f, -1.0f)
    (-1.0f,  1.0f, -1.0f)

    // Top face
    (-1.0f,  1.0f,  1.0f)
    (-1.0f,  1.0f, -1.0f)
    ( 1.0f,  1.0f,  1.0f)
    ( 1.0f,  1.0f, -1.0f)

    // Bottom face
    (-1.0f, -1.0f, -1.0f)
    (-1.0f, -1.0f,  1.0f)
    ( 1.0f, -1.0f, -1.0f)
    ( 1.0f, -1.0f,  1.0f)

    // Right face
    ( 1.0f, -1.0f,  1.0f)
    ( 1.0f,  1.0f,  1.0f)
    ( 1.0f, -1.0f, -1.0f)
    ( 1.0f,  1.0f, -1.0f)

    // Left face
    (-1.0f, -1.0f, -1.0f)
    (-1.0f,  1.0f, -1.0f)
    (-1.0f, -1.0f,  1.0f)
    (-1.0f,  1.0f,  1.0f) |] |> Array.map (fun (x, y, z) -> Vector3(x, y, z))

let normals = [|
    // Front face
    (0.0f,  0.0f,  1.0f)
    (0.0f,  0.0f,  1.0f)
    (0.0f,  0.0f,  1.0f)
    (0.0f,  0.0f,  1.0f)

    // Back face
    (0.0f,  0.0f, -1.0f)
    (0.0f,  0.0f, -1.0f)
    (0.0f,  0.0f, -1.0f)
    (0.0f,  0.0f, -1.0f)

    // Top face
    (0.0f,  1.0f,  0.0f)
    (0.0f,  1.0f,  0.0f)
    (0.0f,  1.0f,  0.0f)
    (0.0f,  1.0f,  0.0f)

    // Bottom face
    (0.0f, -1.0f,  0.0f)
    (0.0f, -1.0f,  0.0f)
    (0.0f, -1.0f,  0.0f)
    (0.0f, -1.0f,  0.0f)

    // Right face
    (1.0f,  0.0f,  0.0f)
    (1.0f,  0.0f,  0.0f)
    (1.0f,  0.0f,  0.0f)
    (1.0f,  0.0f,  0.0f)

    // Left face
    (-1.0f,  0.0f,  0.0f)
    (-1.0f,  0.0f,  0.0f)
    (-1.0f,  0.0f,  0.0f)
    (-1.0f,  0.0f,  0.0f) |] |> Array.map (fun (x, y, z) -> Vector3(x, y, z))

let vertices colour =
    Array.init 24 (fun i -> VertexPositionNormalColour(positions.[i], normals.[i], colour))

let indices = [|
    0;  1;  2;      2;   1;  3;   // front
    4;  5;  6;      6;   5;  7;   // back
    8;  9;  10;     10;  9; 11;   // top
    12; 13; 14;     14; 13; 15;   // bottom
    16; 17; 18;     18; 17; 19;   // right
    20; 21; 22;     22; 21; 23    // left
|]