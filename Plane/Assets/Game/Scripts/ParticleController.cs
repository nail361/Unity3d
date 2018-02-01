using UnityEngine;
public class ParticleController : MonoBehaviour {

    private ParticleSystem ps;

	void Awake () {
        ps = GetComponent<ParticleSystem>();

        if (ps == null)
        {
            Destroy(this);
            return;
        }

        if (!GameManager.instance.SFX)
        {
            Destroy(ps);
            Destroy(this);
        }
	}
}
