using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class SlimeManager : MonoBehaviour {
    private ObiSolver solver;
    private void Start () {
        solver = GetComponentInParent<ObiSolver> ();
        solver.OnCollision += Solver_OnCollision;
    }

    public void Solver_OnCollision (object sender, Obi.ObiSolver.ObiCollisionEventArgs e) {
        var world = ObiColliderWorld.GetInstance ();
        foreach (Oni.Contact contact in e.contacts) {
            ObiColliderBase collider = world.colliderHandles[contact.other].owner;
            if (collider != null) {
                if (collider.tag != "Ground" || collider.tag != "Walls")
                    if (contact.distance < 0.01) {
                        ObiSolver.ParticleInActor pa = solver.particleToActor[contact.particle];
                        Slime s = pa.actor.gameObject.GetComponent<Slime> ();
                        switch (collider.tag) {
                            case "Food":
                                for (var f = 0; f < s.food.Count; f++) {
                                    if (s.food[f] == collider.transform)
                                        if (s.testingFood[f]) {
                                            s.testingFood[f] = false;
                                            s.AddReward (1f);
                                            break;
                                        }
                                }
                                break;
                            case "Enemy":
                                s.Death ();
                                break;
                        }
                    }
            }
        }
    }
}