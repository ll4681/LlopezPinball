# Physics Demo: Tunneling & Flippers (Unity 6)

## Setup (2 minutes)
1. Unzip and drag the **PhysicsDemo** folder into your project's **Assets** folder. Any 2D project works.
2. Wait for scripts to compile, then choose **Tools > Physics Demo > Build Demo Scene**.
3. Set the Game view to **16:9** and press **Play**.

The builder makes everything itself (sprites, physics materials, scene) and saves it as `Assets/PhysicsDemo/PhysicsDemo.unity`. Run it again any time to rebuild a clean scene.

## Controls
**Demo A, Tunneling (left):** Space launch · ↑/↓ speed · C Discrete/Continuous · T move by Transform · R reset
**Demo B, Flippers (right):** ← good flipper · → bad flipper · B drop a ball

## Run of show (about 10 minutes)

**1. It works.** Space at speed 20, Discrete. The ball bounces off the red thin wall forever. Point at the HUD: it jumps 0.40 units per physics step, less than ball + wall (0.60), so every step catches the overlap.

**2. Break it.** Press ↑ a few times to 40–60. The jump number turns red and the Escapes counter starts climbing. The ball isn't going *through* the wall; it's skipping over it. Physics checks 50 times a second, and between checks the ball is simply somewhere else. That's Discrete.

**3. Fix it.** Press C for Continuous. Same speed, and it holds, even at 80. Continuous sweeps the ball's path between steps. It costs more performance, so use it on small fast things (bullets, balls), not everything.

**4. Break the fix.** Press T. The ball is now moved with `transform.position`, which is what most people write first. At low speed it looks fine. Speed it up and it escapes *even though Continuous is on*. Setting the Transform is a teleport, and nothing can sweep a teleport. Rule: move physics objects with velocity or `AddForce`.

**5. Flippers.** Press B to drop balls. The left (green) flipper uses a Kinematic Rigidbody2D and `MoveRotation`, so physics knows it's moving and launches the ball. The right (red) flipper rotates its Transform with no Rigidbody, so it teleports into the ball and the ball gets shoved out of the overlap instead of hit: dribbles, sticks, or passes through.

## The three rules to leave on the board
1. Move physics objects with the Rigidbody (velocity, AddForce, MovePosition/MoveRotation), never the Transform.
2. Small + fast = Continuous. Everything else can stay Discrete.
3. Still tunneling? Cap the speed or make the walls thicker.

## If something looks off
- **Sprites black or invisible:** the builder uses an unlit sprite material, but if your project overrides it, add a Global Light 2D to the scene.
- **Input errors:** the scripts support both the new Input System and the old Input Manager, so this shouldn't happen; if it does, check Project Settings > Player > Active Input Handling.
- **Ball escapes the lane entirely at max speed:** press R. Lower `maxSpeed` on Tunnel Ball if you want to prevent it.
