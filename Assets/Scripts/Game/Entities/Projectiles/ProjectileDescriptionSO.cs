using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDescriptionSO", menuName = "Arena")]
public class ProjectileDescriptionSO : ScriptableObject
{
    public float SpeedMin = 0.0f;
    public float SpeedMax = 0.0f;

    public int Damage = 0;

    public Sprite Sprite = null;

    public Vector2 CollisionHalfExtends = Vector2.zero;
    public Vector2 CollisionOffset = Vector2.zero;
}
