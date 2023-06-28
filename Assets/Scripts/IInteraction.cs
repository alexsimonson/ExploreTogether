using UnityEngine;
namespace ExploreTogether {
    public interface IInteraction{
        void Interaction(GameObject interacting);
        string InteractionName();
    }
}
