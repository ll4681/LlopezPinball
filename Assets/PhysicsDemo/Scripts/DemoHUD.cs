using UnityEngine;

namespace PhysicsDemo
{
    // Big on-screen readout for the projector. OnGUI so there's no UI setup needed.
    public class DemoHUD : MonoBehaviour
    {
        public TunnelBall tunnelBall;
        public int baseFontSize = 24;

        GUIStyle style;

        void OnGUI()
        {
            if (tunnelBall == null) return;
            if (style == null)
                style = new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true };

            float scale = Screen.height / 1080f;
            style.fontSize = Mathf.RoundToInt(baseFontSize * scale);
            float pad = 20f * scale;
            float w = Screen.width * 0.48f;

            var rb = tunnelBall.Body;
            bool continuous = rb.collisionDetectionMode == CollisionDetectionMode2D.Continuous;
            string detection = tunnelBall.TeleportMode
                ? "<color=#888888>(ignored)</color>"
                : (continuous ? "<color=#7CFC8A>CONTINUOUS</color>" : "<color=#FFD84D>DISCRETE</color>");
            string movement = tunnelBall.TeleportMode
                ? "<color=#FF6B6B>transform.position (teleport)</color>"
                : "<color=#7CFC8A>Rigidbody velocity</color>";

            float jump = tunnelBall.DistancePerCheck;
            float danger = tunnelBall.DangerDistance;
            string jumpColor = jump > danger ? "#FF6B6B" : "#7CFC8A";
            string per = tunnelBall.TeleportMode ? "frame" : "physics step";

            string a =
                "<b>DEMO A: TUNNELING</b>\n" +
                $"Collision detection: {detection}\n" +
                $"Moved by: {movement}\n" +
                $"Speed {tunnelBall.speed:0} u/s  →  jumps <color={jumpColor}><b>{jump:0.00}</b></color> units per {per}" +
                $"  (ball + thin wall = {danger:0.00})\n" +
                $"Thin wall hits: <b>{tunnelBall.WallHits}</b>     Escapes: <color=#FF6B6B><b>{tunnelBall.Escapes}</b></color>\n" +
                "<size=" + Mathf.RoundToInt(style.fontSize * 0.8f) + ">" +
                "[Space] launch   [↑/↓] speed   [C] Discrete/Continuous   [T] move by Transform   [R] reset</size>";

            GUI.Label(new Rect(pad, pad, w, Screen.height * 0.3f), a, style);

            string b =
                "<b>DEMO B: FLIPPERS</b>\n" +
                "[←] Left: <color=#7CFC8A>Kinematic Rigidbody2D + MoveRotation</color>\n" +
                "[→] Right: <color=#FF6B6B>rotating the Transform, no Rigidbody</color>\n" +
                "[B] drop another ball";

            GUI.Label(new Rect(pad, Screen.height * 0.72f, w, Screen.height * 0.28f), b, style);
        }
    }
}
