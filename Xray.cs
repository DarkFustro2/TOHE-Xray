using AmongUs.GameOptions;
using System;
using UnityEngine;
using TOHE.Roles.Core;
using TOHE.Modules;
using static TOHE.Options;
using static TOHE.Translator;

namespace TOHE.Roles.Crewmate;

// ============================================================
// XRAY (Crewmate) — TownOfHost-Enhanced Özel Rolü
// ============================================================

public sealed class Xray : RoleBase
{
    // ---- Rol Tanımı ----
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Xray),
            player => new Xray(player),
            CustomRoleTypes.Crewmate,
            CustomRoles.Xray,
            () => RoleTypes.Crewmate,
            new(),
            "#b3f0ff", // Rol rengi (Açık Mavi / X-Ray)
            false, false, false
        );

    private Xray(PlayerControl player) : base(
        RoleInfo,
        player,
        () => DefaultOptionItem
    )
    { }

    // ---- Ayarlanabilir Seçenekler ----
    private static OptionItem CooldownOpt;
    private static OptionItem DurationOpt;
    public static OptionItem DefaultOptionItem;

    public static void SetupCustomOption()
    {
        DefaultOptionItem = SetupRoleOptions(Id: 90001, TabGroup.CrewmateRoles, CustomRoles.Xray);

        CooldownOpt = new FloatOptionItem(90002, "XrayCooldown", new(0f, 60f, 1f), 20f, TabGroup.CrewmateRoles, false)
            .SetParent(DefaultOptionItem)
            .SetValueFormat(OptionFormat.Seconds);

        DurationOpt = new FloatOptionItem(90003, "XrayDuration", new(1f, 10f, 0.5f), 3f, TabGroup.CrewmateRoles, false)
            .SetParent(DefaultOptionItem)
            .SetValueFormat(OptionFormat.Seconds);
    }

    private float Cooldown => CooldownOpt.GetFloat();
    private float Duration => DurationOpt.GetFloat();

    private bool xrayActive = false;
    private float xrayTimer = 0f;

    // Yetenek Butonu Kontrolleri
    public override bool CanUseKillButton(PlayerControl pc) => false;
    public override bool CanUseImpostorVentButton(PlayerControl pc) => false;

    public override void Add(byte playerId)
    {
        AbilityCooldown = Cooldown;
    }

    // Oyuncu butona bastığında çalışır
    public override bool OnCheckAbilityCooldown(PlayerControl pc)
    {
        if (pc == null || pc.Data.IsDead || xrayActive) return false;

        ActivateXray(pc);
        AbilityCooldown = Cooldown; // Cooldown'u yeniden başlatır
        return true;
    }

    private void ActivateXray(PlayerControl pc)
    {
        xrayActive = true;
        xrayTimer = Duration;

        if (pc.AmOwner)
        {
            SetXrayVision(true);
        }

        Utils.NotifyRoles(SpecifySeer: pc, SpecifyTarget: pc);
    }

    public override void OnFixedUpdate(PlayerControl pc)
    {
        if (!xrayActive) return;

        // Toplantı başladıysa veya oyuncu öldüyse efekti hemen kapat
        if (pc == null || pc.Data.IsDead || MeetingHud.Instance != null)
        {
            DeactivateXray(pc);
            return;
        }

        // Zamanlayıcıyı düşür
        xrayTimer -= Time.fixedDeltaTime;

        if (xrayTimer <= 0f)
        {
            DeactivateXray(pc);
        }
    }

    private void DeactivateXray(PlayerControl pc)
    {
        xrayActive = false;
        xrayTimer = 0f;

        if (pc != null && pc.AmOwner)
        {
            SetXrayVision(false);
        }
    }

    /// <summary>
    /// Görüşü artırıp gölgeleri/duvarları geçici olarak devre dışı bırakır.
    /// </summary>
    private void SetXrayVision(bool state)
    {
        try
        {
            if (PlayerControl.LocalPlayer == null) return;

            // 1. Gölgeleri (Duvar engellerini) kapat/aç
            if (HudManager.Instance != null && HudManager.Instance.ShadowService != null)
            {
                HudManager.Instance.ShadowService.gameObject.SetActive(!state);
            }

            // 2. Görüş mesafesini genişlet
            LightSource light = PlayerControl.LocalPlayer.lightSource;
            if (light != null)
            {
                if (state)
                {
                    // X-Ray açıkken haritayı geniş görmesi için büyük bir yarıçap
                    light.viewDistance = 15f; 
                }
                else
                {
                    // X-Ray kapanınca normal görüşe geri dön
                    light.viewDistance = GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.CrewLightMod);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Xray] SetXrayVision hatası: {ex.Message}");
        }
    }
}
