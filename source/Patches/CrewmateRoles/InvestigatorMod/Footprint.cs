using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Extensions;
using TownOfUs.Patches;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    public class Footprint
    {
        public readonly PlayerControl Player;
        private GameObject _gameObject;
        private GameObject _shadowGameObject;
        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer _shadowSpriteRenderer;
        private readonly float _time;
        private readonly Vector2 _velocity;

        public Color Color;
        public Color ShadowColor;
        public Vector3 Position;
        public Investigator Role;

        public Footprint(PlayerControl player, Investigator role)
        {
            Role = role;
            Position = player.transform.position;
            _velocity = player.gameObject.GetComponent<Rigidbody2D>().velocity;

            Player = player;
            _time = (int) Time.time;
            Color = Color.black;
            ShadowColor = Color.black;

            Start();
            role.AllPrints.Add(this);
        }

        public static float Duration => CustomGameOptions.FootprintDuration;

        public static bool Grey =>
            CustomGameOptions.AnonymousFootPrint || CamouflageUnCamouflage.IsCamoed;

        public static void DestroyAll(Investigator role)
        {
            while (role.AllPrints.Count != 0) role.AllPrints[0].Destroy();
        }

        private void Start()
        {
            _gameObject = new GameObject("Footprint");
            _gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            _gameObject.transform.position = Position;
            _gameObject.transform.Rotate(Vector3.forward * Vector2.SignedAngle(Vector2.up, _velocity));
            _gameObject.transform.SetParent(Player.transform.parent);

            _shadowGameObject = new GameObject("FootprintShadow");
            _shadowGameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            _shadowGameObject.transform.position = Position;
            _shadowGameObject.transform.Rotate(Vector3.forward * Vector2.SignedAngle(Vector2.up, _velocity));
            _shadowGameObject.transform.SetParent(Player.transform.parent);

            _spriteRenderer = _gameObject.AddComponent<SpriteRenderer>();
            _spriteRenderer.sprite = TownOfUs.Footprint;
            _spriteRenderer.color = Color;

            _shadowSpriteRenderer = _shadowGameObject.AddComponent<SpriteRenderer>();
            _shadowSpriteRenderer.sprite = TownOfUs.FootprintShadow;
            _shadowSpriteRenderer.color = ShadowColor;

            _gameObject.transform.localScale *= new Vector2(1.2f, 1f) * (CustomGameOptions.FootprintSize / 10);
            _shadowGameObject.transform.localScale *= new Vector2(1.2f, 1f) * (CustomGameOptions.FootprintSize / 10);

            _gameObject.SetActive(true);
            _shadowGameObject.SetActive(true);
        }

        private void Destroy()
        {
            Object.Destroy(_shadowGameObject);
            Object.Destroy(_gameObject);
            Role.AllPrints.Remove(this);
        }

        public bool Update()
        {
            var currentTime = Time.time;
            var alpha = Mathf.Max(1f - (currentTime - _time) / Duration, 0f);

            if (alpha < 0 || alpha > 1)
                alpha = 0;
            
            if (Grey)
            {
                Color = new Color(0.2f, 0.2f, 0.2f, 1f);
                ShadowColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            }
            else if (RainbowUtils.IsRainbow(Player.GetDefaultOutfit().ColorId))
            {
                Color = RainbowUtils.Rainbow;
                ShadowColor = RainbowUtils.RainbowShadow;
            }
            else if (RainbowUtils.IsGrayscale(Player.GetDefaultOutfit().ColorId))
            {
                Color = RainbowUtils.Grayscale;
                ShadowColor = RainbowUtils.GrayscaleShadow;
            }
            else if (RainbowUtils.IsFire(Player.GetDefaultOutfit().ColorId))
            {
                Color = RainbowUtils.Fire;
                ShadowColor = RainbowUtils.FireShadow;
            }
            else if (RainbowUtils.IsGalaxy(Player.GetDefaultOutfit().ColorId))
            {
                Color = RainbowUtils.Galaxy;
                ShadowColor = RainbowUtils.GalaxyShadow;
            }
            else
            {
                Color = Palette.PlayerColors[Player.GetDefaultOutfit().ColorId];
                ShadowColor = Palette.ShadowColors[Player.GetDefaultOutfit().ColorId];
            }

            Color = new Color(Color.r, Color.g, Color.b, alpha);
            ShadowColor = new Color(ShadowColor.r, ShadowColor.g, ShadowColor.b, alpha);
            _spriteRenderer.color = Color;
            _shadowSpriteRenderer.color = ShadowColor;

            if (_time + (int) Duration < currentTime)
            {
                Destroy();
                return true;
            }

            return false;
        }
    }
}