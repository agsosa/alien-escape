using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Optimizar y solo sincronizar las variables necesarias

public class RemoteSettings: MonoBehaviourSingleton<RemoteSettings>
{
    [Header("GameSceneManager Settings")]
    public float MAX_SECONDS_PER_GAME = 60;
    public int MAX_INVADERS = 6; // Numero de invaders inicial
    public int MAX_ALIENS = 1;
    public int PLAYER_DRINKING_FREEZE_MILLISECONDS = 1000; // Milliseconds
    public int PLAYER_INITIAL_DRINKS_PER_GAME = 3;
    public int PLAYER_INITIAL_MED_KITS_PER_GAME = 1;

    [Header("Score system")]
    public int SCORE_GAIN_PER_SAVED_ALIEN = 150;
    public float TIME_GAIN_PER_SAVED_ALIEN = 10;
    public int SCORE_GAIN_PER_KILLED_SOLDIER = 300;
    public float TIME_GAIN_PER_KILLED_SOLDIER = 5;
    public int SCORE_GAIN_PER_STOLEN_BRIEFCASE = 150;
    public float TIME_GAIN_PER_STOLEN_BRIEFCASE = 5;

    [Header("Player Character Settings (shared Player & Invader controllers)")] // Estos valores de daño se iran multiplicando linearmente por DAMAGE_SCALING_MULTIPLIER_MAX
    public float PLAYER_CHARACTER_BULLET_DAMAGE_MIN = 0.55f; // Random entre Min 
    public float PLAYER_CHARACTER_BULLET_DAMAGE_MAX = 1.55f; // Random entre Min y Max (daño recibido por bala)
    public float PLAYER_CHARACTER_MINE_EXPLOSION_DAMAGE = 25;
    public float PLAYER_CHARACTER_MISIL_EXPLOSION_DAMAGE = 40;
    public float PLAYER_CHARACTER_MAX_HEALTH = 100; // Aplica tanto para player como para invader. Vida maxima
    public float PLAYER_CHARACTER_MIN_HEAL_AMOUNT = 100;
    public float PLAYER_CHARACTER_MAX_HEAL_AMOUNT = 100;
    public float PLAYER_CHARACTER_HEAL_INVULNERABILITY_TIME = 5;// Tiempo (segundos) que sera invulnerable un PlayerCharacter luego de haber sido curado

    [Header("DMG Scaling system")] // Sistema de escalado de daño por tiempo y/o score
    public int DAMAGE_SCALING_SCORE_HARD_CAP = 7500; // Este sera el maximo score hasta que termine de aumentar el daño
    public float GetLinearDamageScalingMultiplier()
    {
        if (GameSceneManager.Instance == null) return 1;
        float lerpValue = (float)GameSceneManager.Instance.GameState.Score / (float)DAMAGE_SCALING_SCORE_HARD_CAP;
     //   Debug.LogError("Current damage multiplier = " + Mathf.Lerp(1, DAMAGE_SCALING_MULTIPLIER_MAX, lerpValue));
        return Mathf.Lerp(1, DAMAGE_SCALING_MULTIPLIER_MAX, lerpValue);
    }
    public float DAMAGE_SCALING_MULTIPLIER_MAX = 5;

    [Header("Player Character Energy system")]
    public float PLAYER_CHARACTER_ACTIVATE_ENERGY_FREEZE_TIME = 1f; // Tiempo que se congelara el personaje al activar el modo god
    public bool PLAYER_CHARACTER_ENERGY_INVULNERABILITY = true; // Invulnerabilidad durante el god mode
    public float PLAYER_CHARACTER_ENERGY_SPEED_MODIFIER = 2; // Multiplicador de velocidad en el god mode
    public float PLAYER_CHARACTER_ACTIVE_ENERGY_DECREASE_AMOUNT = 2.33f; // Monto que se quitara cada DECREASE_RATE al tener activado el modo god
    public float PLAYER_CHARACTER_ACTIVE_ENERGY_DECREASE_AMOUNT_MIN = 1.33f; // Monto minimo al que descendera el decrease amount segun el SKin Power
    public float PLAYER_CHARACTER_ACTIVE_ENERGY_DECREASE_RATE = 0.4f; // Rate en el que se aplicara el DECREASE_AMOUNT

    [Header("Player Character Energy System Gain")]
    public float PLAYER_CHARACTER_ENERGY_REGENERATION_WAIT_SECONDS_BEFORE_START = 0.1f; // Tiempo de espera entre cada regenreacion (antes de q empiece el juego)
    public float PLAYER_CHARACTER_ENERGY_REGENERATION_AMOUNT_BEFORE_START = 5; // Cantidad que se regenerara cada wait seconds (antes de q empiece el juego)
    public float PLAYER_CHARACTER_ENERGY_REGENERATION_WAIT_SECONDS = 0.3f; // Tiempo de espera entre cada regenreacion
    public float PLAYER_CHARACTER_ENERGY_REGENERATION_AMOUNT = 3; // Cantidad que se regenerara cada wait seconds
    public float PLAYER_CHARACTER_ENERGY_PER_DAMAGE_MIN = 0.3f; // Energia al obtener daño (min)
    public float PLAYER_CHARACTER_ENERGY_PER_DAMAGE_MAX = 0.75f; // Energia al obtener daño (max)
    public float PLAYER_CHARACTER_ENERGY_PER_DAMAGE_MULTIPLIER = 1f; // Este sera el valor que se multiplique al daño luego de calcular min y max
    public float PLAYER_CHARACTER_ENERGY_PER_DEATH = 2.0f; // Multiplicador al morirse un invader (lo multiplica al energy gain calculado apartir de los dos valores de arriba

    [Header("Invader Settings")]
    public float INVADER_PLAYER_SPRINTING_SPEED = 8; // Velocidad que usara cuando el jugador sprintee
    public float INVADER_PLAYER_NORMAL_RUN_SPEED = 4; // Velocidad que usara cuando el jugador no sprintee
    public float INVADER_ROTATION_SPEED = 100; // Velocidad a la que rotara el invader
    public float INVADER_FOLLOW_PLAYER_STOP_DISTANCE = 4;
    public float INVADER_ANIM_LERP_SPEED = 5;
    public float INVADER_MIN_MAGNITUDE_MOVING = 2;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
