
// Interface for taking damage, implemented by the enemies and the player
// in PlayerLifeCycle.cs and EnemyLifeCycle.cs
public interface IDamageable {

    void TakeDamage(float damage);
}
