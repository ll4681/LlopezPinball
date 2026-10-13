using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PhysicsDemo.EditorTools
{
    // One click: Tools > Physics Demo > Build Demo Scene
    // Generates sprites, physics materials and the whole scene, then saves it.
    public static class PhysicsDemoBuilder
    {
        const string Root = "Assets/PhysicsDemo";
        const string Gen = Root + "/Generated";
        const string ScenePath = Root + "/PhysicsDemo.unity";

        static Sprite square, circle;
        static Material spriteMat;

        static readonly Color WallColor = new Color(0.55f, 0.6f, 0.7f);
        static readonly Color Red = new Color(1f, 0.3f, 0.3f);
        static readonly Color Green = new Color(0.45f, 0.95f, 0.55f);
        static readonly Color Yellow = new Color(1f, 0.85f, 0.3f);

        [MenuItem("Tools/Physics Demo/Build Demo Scene")]
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            if (!AssetDatabase.IsValidFolder(Root)) AssetDatabase.CreateFolder("Assets", "PhysicsDemo");
            if (!AssetDatabase.IsValidFolder(Gen)) AssetDatabase.CreateFolder(Root, "Generated");

            square = MakeSprite(Gen + "/Square.png", false);
            circle = MakeSprite(Gen + "/Circle.png", true);
            spriteMat = MakeSpriteMaterial(Gen + "/DemoSprite.mat");
            var bouncy = MakePhysMat(Gen + "/Bouncy.physicsMaterial2D", 1f, 0f);
            var pinballMat = MakePhysMat(Gen + "/Pinball.physicsMaterial2D", 0.3f, 0.2f);
            AssetDatabase.SaveAssets();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            camGO.transform.position = new Vector3(0f, 0f, -10f);
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 10.5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.09f, 0.12f);

            // ---------- DEMO A: tunneling lane (left half) ----------
            var areaA = new GameObject("Demo A - Tunneling").transform;
            Box("Lane Top", areaA, new Vector2(-9.5f, 3.25f), new Vector2(17f, 0.5f), 0f, WallColor);
            Box("Lane Bottom", areaA, new Vector2(-9.5f, -3.25f), new Vector2(17f, 0.5f), 0f, WallColor);
            Box("Backstop (thick)", areaA, new Vector2(-17.5f, 0f), new Vector2(4f, 7f), 0f, WallColor);
            Box("Catcher (thick)", areaA, new Vector2(-1.5f, 0f), new Vector2(2f, 7f), 0f, WallColor);
            Box("ThinWall", areaA, new Vector2(-5f, 0f), new Vector2(0.1f, 6f), 0f, Red);

            var zone = Box("Escape Zone (visual only)", areaA, new Vector2(-3.75f, 0f), new Vector2(2.4f, 6f), 0f,
                new Color(1f, 0.3f, 0.3f, 0.12f));
            Object.DestroyImmediate(zone.GetComponent<BoxCollider2D>());

            var ball = new GameObject("Tunnel Ball");
            ball.transform.SetParent(areaA, false);
            ball.transform.localPosition = new Vector3(-12f, 0f, 0f);
            ball.transform.localScale = Vector3.one * 0.5f;
            var bsr = ball.AddComponent<SpriteRenderer>();
            bsr.sprite = circle;
            bsr.color = Yellow;
            if (spriteMat != null) bsr.sharedMaterial = spriteMat;
            var bcol = ball.AddComponent<CircleCollider2D>();
            bcol.radius = 0.5f;
            bcol.sharedMaterial = bouncy;
            var brb = ball.AddComponent<Rigidbody2D>();
            brb.gravityScale = 0f;
            brb.collisionDetectionMode = CollisionDetectionMode2D.Discrete; // start in the "broken" mode
            var tunnel = ball.AddComponent<TunnelBall>();
            tunnel.thinWallName = "ThinWall";
            tunnel.thinWallX = -5f;
            tunnel.thinWallThickness = 0.1f;

            // ---------- DEMO B: flippers (right half) ----------
            var areaB = new GameObject("Demo B - Flippers").transform;
            Box("Pit Left", areaB, new Vector2(1.5f, 0f), new Vector2(1f, 20f), 0f, WallColor);
            Box("Pit Right", areaB, new Vector2(16.5f, 0f), new Vector2(1f, 20f), 0f, WallColor);
            Box("Pit Top", areaB, new Vector2(9f, 9.75f), new Vector2(16f, 0.5f), 0f, WallColor);
            Box("Drain", areaB, new Vector2(9f, -9.75f), new Vector2(16f, 0.5f), 0f, new Color(0.3f, 0.3f, 0.35f));
            Between("Guide Left", areaB, new Vector2(2f, -2.3f), new Vector2(5.3f, -4.85f), 0.4f, WallColor);
            Between("Guide Right", areaB, new Vector2(16f, -2.3f), new Vector2(12.7f, -4.85f), 0.4f, WallColor);

            // Good flipper: Kinematic Rigidbody2D + MoveRotation
            var good = FlipperPivot("Flipper L - Rigidbody2D + MoveRotation (GOOD)", areaB,
                new Vector2(5.5f, -5f), -25f, true, Green);
            var grb = good.AddComponent<Rigidbody2D>();
            grb.bodyType = RigidbodyType2D.Kinematic;
            grb.interpolation = RigidbodyInterpolation2D.Interpolate;
            var gf = good.AddComponent<PhysicsFlipper>();
            gf.key = KeyCode.LeftArrow;
            gf.restAngle = -25f;
            gf.activeAngle = 25f;

            // Bad flipper: no Rigidbody, rotate the Transform
            var bad = FlipperPivot("Flipper R - Transform rotation (BAD)", areaB,
                new Vector2(12.5f, -5f), 25f, false, Red);
            var bf = bad.AddComponent<TransformFlipper>();
            bf.key = KeyCode.RightArrow;
            bf.restAngle = 25f;
            bf.activeAngle = -25f;

            var spawner = areaB.gameObject.AddComponent<BallSpawner>();
            spawner.ballSprite = circle;
            spawner.spriteMaterial = spriteMat;
            spawner.ballMaterial = pinballMat;
            spawner.ballColor = Color.white;

            // HUD
            var hud = new GameObject("HUD").AddComponent<DemoHUD>();
            hud.tunnelBall = tunnel;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            Debug.Log("Physics demo scene built at " + ScenePath + ". Set the Game view to 16:9 and press Play.");
        }

        // ---------- helpers ----------

        static GameObject Box(string name, Transform parent, Vector2 center, Vector2 size, float angle, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = center;
            go.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = square;
            sr.color = color;
            if (spriteMat != null) sr.sharedMaterial = spriteMat;
            var col = go.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            col.offset = Vector2.zero;
            return go;
        }

        static GameObject Between(string name, Transform parent, Vector2 a, Vector2 b, float thickness, Color color)
        {
            Vector2 d = b - a;
            float angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
            return Box(name, parent, (a + b) * 0.5f, new Vector2(d.magnitude, thickness), angle, color);
        }

        static GameObject FlipperPivot(string name, Transform parent, Vector2 pivot, float restAngle,
            bool pivotOnLeft, Color color, float length = 3f, float thickness = 0.35f)
        {
            var p = new GameObject(name);
            p.transform.SetParent(parent, false);
            p.transform.localPosition = pivot;
            p.transform.localRotation = Quaternion.Euler(0f, 0f, restAngle);
            Box("Paddle", p.transform, new Vector2(pivotOnLeft ? length / 2f : -length / 2f, 0f),
                new Vector2(length, thickness), 0f, color);

            var cap = new GameObject("Pivot");
            cap.transform.SetParent(p.transform, false);
            cap.transform.localScale = Vector3.one * 0.5f;
            var sr = cap.AddComponent<SpriteRenderer>();
            sr.sprite = circle;
            sr.color = Color.white;
            sr.sortingOrder = 1;
            if (spriteMat != null) sr.sharedMaterial = spriteMat;
            return p;
        }

        static Sprite MakeSprite(string assetPath, bool isCircle)
        {
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var px = new Color32[size * size];
            float r = size / 2f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                byte alpha = 255;
                if (isCircle)
                {
                    float dx = x + 0.5f - r, dy = y + 0.5f - r;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    alpha = (byte)(Mathf.Clamp01(r - dist) * 255f);
                }
                px[y * size + x] = new Color32(255, 255, 255, alpha);
            }
            tex.SetPixels32(px);
            tex.Apply();
            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), assetPath), tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            var imp = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.spritePixelsPerUnit = size; // 64px = 1 world unit
            imp.mipmapEnabled = false;
            imp.alphaIsTransparency = true;
            imp.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        // Unlit sprite material so sprites show up even in a URP 2D project with no lights.
        static Material MakeSpriteMaterial(string assetPath)
        {
            var existing = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (existing != null) return existing;
            var shader = Shader.Find("Sprites/Default");
            if (shader == null) return null;
            var mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, assetPath);
            return mat;
        }

        static PhysicsMaterial2D MakePhysMat(string assetPath, float bounciness, float friction)
        {
            var mat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(assetPath);
            if (mat == null)
            {
                mat = new PhysicsMaterial2D();
                AssetDatabase.CreateAsset(mat, assetPath);
            }
            mat.bounciness = bounciness;
            mat.friction = friction;
            EditorUtility.SetDirty(mat);
            return mat;
        }
    }
}
