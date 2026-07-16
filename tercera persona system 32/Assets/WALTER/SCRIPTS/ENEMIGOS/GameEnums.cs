// Archivo central de enums del proyecto. Todos los enums globales deben definirse aquí.

public enum TipoEnemigo
{
    Alien,
    Nave
}

public enum WeaponType
{
    Melee,
    Firearm
}

public enum TipoAudio
{
    Efectos,
    Musica
}

public enum TipoConsumible
{
    CuracionInstantanea
}

// Tipos de munición del juego. Cada arma de fuego usa uno,
// y los ítems de munición de las máquinas venden por tipo.
public enum TipoMunicion
{
    Pistola,
    Metralleta,
    Escopeta,
    Rifle
}

public enum TipoMejora
{
    VidaMaxima,
    EscudoMaximo,
    VelocidadRegenEscudo,
    VelocidadMovimiento,
    CadenciaDisparo,
    CapacidadMunicion,
    VelocidadRecarga,
    Danio
}
