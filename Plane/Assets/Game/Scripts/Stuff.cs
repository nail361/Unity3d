using UnityEngine;

public class Stuff : MonoBehaviour {

	public bool isGood;
	public bool isFast = false;
	public bool badEffect;
	public bool fuel;

	public Vector2 location;

	public enum GoodBadTypes{ Fast = -13, Invert = -12, HideSetka = -11, SpaceshipLong = -3, Asteroid = -2, Spaceship = -1, Point = 0, Shield = 1, Magnet = 2, Slow = 3, Fuel = 10 };
	public GoodBadTypes type;

	private bool zero = false;
	private bool magnet = false;
	public Transform PlayerTransform;

	public void setType( int value ){

		if (!isGood) value *=-1;
		if (badEffect) value -= 10;

		if (GoodBadTypes.IsDefined(typeof(GoodBadTypes), value))
			type = (GoodBadTypes)value;
		else{
			Debug.LogError("NO STUFF TYPE");
			LevelManager.instance.DieStuff( gameObject );
		}

	}

	void OnTriggerEnter(Collider trigger){
		if (trigger.tag == "Fast"){
			if ( type != GoodBadTypes.Point && !zero )
				LevelManager.instance.SetkaZero(location);
			LevelManager.instance.DieStuff( gameObject );
		}
		else if (trigger.tag == "Front" && type != GoodBadTypes.Point && type != GoodBadTypes.Fuel ){
			zero = true;
			LevelManager.instance.SetkaZero(location);
		}
		else if (trigger.tag == "Die") LevelManager.instance.DieStuff( gameObject );
		else if (trigger.tag == "Magnet" && type == GoodBadTypes.Point && !magnet ){
			magnet = true;
		}
		else if (trigger.tag == "Shield" && (int)type < 0 ){
			LevelManager.instance.DieStuff( gameObject, true );
		}
		else if (trigger.tag == "Player"){

			switch(type){
				case GoodBadTypes.Fast: LevelManager.instance.PickFast((int)type); break;
				case GoodBadTypes.HideSetka: LevelManager.instance.PickHideSetka((int)type); break;
				case GoodBadTypes.Invert: LevelManager.instance.PickInvert((int)type); break;
				case GoodBadTypes.Asteroid:
				case GoodBadTypes.Spaceship:
                case GoodBadTypes.SpaceshipLong:
                    LevelManager.instance.PlayerDie();
				break;

				case GoodBadTypes.Point: LevelManager.instance.PickPoint(); break;
				case GoodBadTypes.Shield: LevelManager.instance.PickShield((int)type); break;
				case GoodBadTypes.Magnet: LevelManager.instance.PickMagnet((int)type); break;
				case GoodBadTypes.Slow: LevelManager.instance.PickSlow((int)type); break;
				case GoodBadTypes.Fuel: LevelManager.instance.PickFuel(); break;

			default: break;

			}

			LevelManager.instance.DieStuff( gameObject );
		}
	}

	void Update () {

		float curSpeed = LevelManager.instance.GetSpeed( isFast );

		if (magnet){
			transform.position = Vector3.Lerp( transform.position, PlayerTransform.position, curSpeed * Time.deltaTime );
		}
		else
			transform.Translate( Vector3.forward * curSpeed * Time.deltaTime );

	}
}
