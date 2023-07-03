using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(DecisionRequester))]
public class NNTrack : Agent
{
    [System.Serializable]
    public class RewardInfo
    {
        public float forwardMultiplier = 0.001f;  // Wspó³czynnik nagrody za poruszanie siê do przodu
        public float barrierMultiplier = -0.8f;  // Wspó³czynnik kary za kolizjê z przeszkod¹
        public float carMultiplier = -0.5f;      // Wspó³czynnik kary za kolizjê z innym samochodem
    }

    public float moveSpeed = 30;                 // Prêdkoœæ poruszania siê samochodu
    public float turnSpeed = 80;                 // Prêdkoœæ obracania samochodu
    public RewardInfo rewardSettings = new RewardInfo();
    public bool doEpisodes = true;

    private Rigidbody rb;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Bounds bounds;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = 1;
        rb.angularDrag = 5;
        rb.interpolation = RigidbodyInterpolation.Extrapolate;

        GetComponent<MeshCollider>().convex = true;
        GetComponent<DecisionRequester>().DecisionPeriod = 1;
        bounds = GetComponent<MeshRenderer>().bounds;

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!IsWheelsDown())
            return;

        float velocityMagnitude = Mathf.Abs(rb.velocity.sqrMagnitude);

        switch (actions.DiscreteActions.Array[0])   // Ruch
        {
            case 0:
                // Brak akcji, samochód siê nie porusza
                break;
            case 1:
                // Akcja wstecz - dodanie si³y do ty³u
                rb.AddRelativeForce(Vector3.back * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
                break;
            case 2:
                // Akcja do przodu - dodanie si³y do przodu i nagroda za poruszanie siê do przodu
                rb.AddRelativeForce(Vector3.forward * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
                AddReward(velocityMagnitude * rewardSettings.forwardMultiplier);
                break;
        }

        switch (actions.DiscreteActions.Array[1])   // Obrót
        {
            case 0:
                // Brak akcji, samochód siê nie obraca
                break;
            case 1:
                // Akcja obrót w lewo
                transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
                break;
            case 2:
                // Akcja obrót w prawo
                transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
                break;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        actionsOut.DiscreteActions.Array[0] = 0;
        actionsOut.DiscreteActions.Array[1] = 0;

        float move = Input.GetAxis("Vertical");     // -1..0..1  WASD arrowkeys
        float turn = Input.GetAxis("Horizontal");

        if (move < 0)
            actionsOut.DiscreteActions.Array[0] = 1;    // Wstecz
        else if (move > 0)
            actionsOut.DiscreteActions.Array[0] = 2;    // Naprzód

        if (turn < 0)
            actionsOut.DiscreteActions.Array[1] = 1;    // W lewo
        else if (turn > 0)
            actionsOut.DiscreteActions.Array[1] = 2;    // W prawo
    }

    private void OnCollisionEnter(Collision collision)
    {
        float collisionMagnitude = collision.relativeVelocity.sqrMagnitude;

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * collisionMagnitude, Color.red, 1f);
        }

        if (collision.gameObject.CompareTag("BarrierWhite") || collision.gameObject.CompareTag("BarrierYellow"))
        {
            // Kolizja z przeszkod¹ - kara i zakoñczenie epizodu
            AddReward(collisionMagnitude * rewardSettings.barrierMultiplier);
            if (doEpisodes)
                EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Car"))
        {
            // Kolizja z innym samochodem - kara i zakoñczenie epizodu
            AddReward(collisionMagnitude * rewardSettings.carMultiplier);
            if (doEpisodes)
                EndEpisode();
        }
    }

    private bool IsWheelsDown()
    {
        // Sprawdzenie, czy ko³a s¹ na ziemi
        return Physics.Raycast(transform.position, -transform.up, bounds.size.y * 0.55f);
    }
}

/*

using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(DecisionRequester))]
public class NNTrack : Agent
{
    [System.Serializable]
    public class RewardInfo
    {
        public float mult_forward = 0.001f;
        public float mult_barrier = -0.8f;
        public float mult_car = -0.5f;
    }

    public float MoveSpeed = 30;
    public float TurnSpeed = 80;
    public RewardInfo RewardSettings = new RewardInfo();
    public bool DoEpisodes = true;

    private Rigidbody rb;
    private Vector3 recallPosition;
    private Quaternion recallRotation;
    private Bounds bounds;

    public override void Initialize()
    {
        // Inicjalizacja komponentów i ustawienie pocz¹tkowych wartoœci

        rb = GetComponent<Rigidbody>();
        rb.drag = 1;
        rb.angularDrag = 5;
        rb.interpolation = RigidbodyInterpolation.Extrapolate;

        GetComponent<MeshCollider>().convex = true;
        GetComponent<DecisionRequester>().DecisionPeriod = 1;
        bounds = GetComponent<MeshRenderer>().bounds;

        recallPosition = transform.position;
        recallRotation = transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        // Resetowanie wartoœci na pocz¹tku ka¿dego epizodu

        rb.velocity = Vector3.zero;
        transform.position = recallPosition;
        transform.rotation = recallRotation;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Wykonanie akcji na podstawie otrzymanych decyzji

        if (!IsWheelsDown())
            return;

        float velocityMagnitude = Mathf.Abs(rb.velocity.sqrMagnitude);

        switch (actions.DiscreteActions.Array[0])   // Ruch
        {
            case 0:
                break;
            case 1:
                rb.AddRelativeForce(Vector3.back * MoveSpeed * Time.deltaTime, ForceMode.VelocityChange); // Wstecz
                break;
            case 2:
                rb.AddRelativeForce(Vector3.forward * MoveSpeed * Time.deltaTime, ForceMode.VelocityChange); // Naprzód
                AddReward(velocityMagnitude * RewardSettings.mult_forward);
                break;
        }

        switch (actions.DiscreteActions.Array[1])   // Obrót
        {
            case 0:
                break;
            case 1:
                transform.Rotate(Vector3.up, -TurnSpeed * Time.deltaTime); // W lewo
                break;
            case 2:
                transform.Rotate(Vector3.up, TurnSpeed * Time.deltaTime); // W prawo
                break;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Symulacja akcji za pomoc¹ klawiatury

        actionsOut.DiscreteActions.Array[0] = 0;
        actionsOut.DiscreteActions.Array[1] = 0;

        float move = Input.GetAxis("Vertical");     // -1..0..1  WASD arrowkeys
        float turn = Input.GetAxis("Horizontal");

        if (move < 0)
            actionsOut.DiscreteActions.Array[0] = 1;    // Wstecz
        else if (move > 0)
            actionsOut.DiscreteActions.Array[0] = 2;    // Naprzód

        if (turn < 0)
            actionsOut.DiscreteActions.Array[1] = 1;    // W lewo
        else if (turn > 0)
            actionsOut.DiscreteActions.Array[1] = 2;    // W prawo
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Obs³uga kolizji

        float collisionMagnitude = collision.relativeVelocity.sqrMagnitude;

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * collisionMagnitude, Color.red, 1f);
        }

        if (collision.gameObject.CompareTag("BarrierWhite") || collision.gameObject.CompareTag("BarrierYellow"))
        {
            AddReward(collisionMagnitude * RewardSettings.mult_barrier);
            if (DoEpisodes)
                EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Car"))
        {
            AddReward(collisionMagnitude * RewardSettings.mult_car);
            if (DoEpisodes)
                EndEpisode();
        }
    }

    private bool IsWheelsDown()
    {
        // Sprawdzenie, czy ko³a s¹ na ziemi

        return Physics.Raycast(transform.position, -transform.up, bounds.size.y * 0.55f);
    }
}
*/