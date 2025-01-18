using UnityEngine;

public class BulletManager : MonoBehaviour
{
	[SerializeField] private GameObject _bullet;
	[SerializeField] private Transform _spawnPoint;
	[SerializeField] private BulletPropertySelector _propertySelector;

	private void OnEnable()
	{
		PlayerController.E_PlayerShoot += HandlePlayerShoot;
	}

	private void OnDisable()
	{
		PlayerController.E_PlayerShoot -= HandlePlayerShoot;
	}

	private void HandlePlayerShoot()
	{
		var shape = _propertySelector.SelectedShape;
		var color = _propertySelector.SelectedColor;
		var outline = _propertySelector.SelectedOutline;

		GameObject bulletInstance = Instantiate(_bullet, _spawnPoint.position, _spawnPoint.rotation);

		BulletController bullet = bulletInstance.GetComponent<BulletController>();
		bullet.InitializeBullet(shape, color, outline);

		//Debug.Log($"Fired Bulet: Shape = {shape}, Base Color = {color}, Outline = {outline}");
	}
}
