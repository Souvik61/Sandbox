using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Animated leaderboard where rows animate to final position
/// </summary>
public class DynamicLeaderboard : MonoBehaviour
{
    /// <summary>
    /// Represents a placing
    /// </summary>
    public class Placing
    {
        public int Position;
        public string Name;
        public int PlayerIndex;
        public bool IsPlayer;
        public float Score;

        public override bool Equals(object _obj)
        {
            return _obj != null && _obj is Placing placing &&
                placing.Position == Position && placing.Name == Name && placing.Score == Score;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public int listingRowCount;

    [SerializeField]
    [Tooltip("The container for the list of 'placing' entries")]
    private Transform placingList;

    private Placing[] currentPlacings = new Placing[10];

    private Placing[] newPlacings = new Placing[10];

    private Placing[] dummyPlacings;

    private Placing[] dummyPlacings1;

    bool isAnimating;

    Vector3[] lerpStartPlacingLocalPositions;

    private void Awake()
    {
        //currentPlacings = new Placing[listingRowCount];

        lerpStartPlacingLocalPositions = new Vector3[listingRowCount];
    }

// Start is called before the first frame update
void Start()
    {
        dummyPlacings = new Placing[10];
        dummyPlacings1 = new Placing[10];

        //Initial placing
        dummyPlacings[0] = new Placing() { PlayerIndex = 0, Position = 0, Name = "A", Score = 1 };
        dummyPlacings[1] = new Placing() { PlayerIndex = 1, Position = 1, Name = "B", Score = 2 };
        dummyPlacings[2] = new Placing() { PlayerIndex = 2, Position = 2, Name = "C", Score = 5 };
        dummyPlacings[3] = new Placing() { PlayerIndex = 3, Position = 3, Name = "D", Score = 4 };
        dummyPlacings[4] = new Placing() { PlayerIndex = 4, Position = 4, Name = "E", Score = 5 };
        dummyPlacings[5] = new Placing() { PlayerIndex = 5, Position = 5, Name = "F", Score = 44 };
        dummyPlacings[6] = new Placing() { PlayerIndex = 6, Position = 6, Name = "G", Score = 78 };
        dummyPlacings[7] = new Placing() { PlayerIndex = 7, Position = 7, Name = "H", Score = 88 };
        dummyPlacings[8] = new Placing() { PlayerIndex = 8, Position = 8, Name = "I", Score = 4 };
        dummyPlacings[9] = new Placing() { PlayerIndex = 9, Position = 9, Name = "J", Score = 100 };

        //Target placing for animation
        dummyPlacings1[9] = new Placing() { PlayerIndex = 0, Position = 0, Name = "A", Score = 1 };
        dummyPlacings1[4] = new Placing() { PlayerIndex = 1, Position = 1, Name = "B", Score = 2 };
        dummyPlacings1[2] = new Placing() { PlayerIndex = 2, Position = 2, Name = "C", Score = 5 };
        dummyPlacings1[3] = new Placing() { PlayerIndex = 3, Position = 3, Name = "D", Score = 4 };
        dummyPlacings1[1] = new Placing() { PlayerIndex = 4, Position = 4, Name = "E", Score = 5 };
        dummyPlacings1[7] = new Placing() { PlayerIndex = 5, Position = 5, Name = "F", Score = 44 };
        dummyPlacings1[6] = new Placing() { PlayerIndex = 6, Position = 6, Name = "G", Score = 78 };
        dummyPlacings1[5] = new Placing() { PlayerIndex = 7, Position = 7, Name = "H", Score = 88 };
        dummyPlacings1[8] = new Placing() { PlayerIndex = 8, Position = 8, Name = "I", Score = 4 };
        dummyPlacings1[0] = new Placing() { PlayerIndex = 9, Position = 9, Name = "J", Score = 100 };

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Set leaderboard");
            Set(dummyPlacings);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Set leaderboard animated");
            SetAnimated(dummyPlacings1);
        }
    }

    public Placing[] Get()
    {
        return currentPlacings;
    }

    /// <summary>
    /// Set row to given placing data
    /// </summary>
    /// <param name="row"></param>
    /// <param name="placingData"></param>
    void SetRowDisplayData(Transform row, Placing placingData)
    {
        //Retrieve inner row object
        row.transform.GetChild(0).localPosition = Vector3.zero;
        TextMeshProUGUI name = row.FindDeep("Txt Name").GetComponent<TextMeshProUGUI>();
        name.text = placingData.Name;
        TextMeshProUGUI score = row.FindDeep("Txt Score").GetComponent<TextMeshProUGUI>();
        score.enabled = true;
        score.text = $"{placingData.Score:0.00}";
        row.FindDeep("Txt Rank").GetComponent<TextMeshProUGUI>().text = (placingData.Position + 1).ToString();

        //IsPlayer logic
        //if (pEntry.IsPlayer)
        //{
        //    row.FindDeep("BG_Player").gameObject.SetActive(true);
        //    row.FindDeep("BG_Opponent").gameObject.SetActive(false);
        //}
        //else
        //{
        //    row.FindDeep("BG_Player").gameObject.SetActive(false);
        //    row.FindDeep("BG_Opponent").gameObject.SetActive(true);
        //    name.color = opponentColor;
        //}
    }

    /// <summary>
    /// Set the leaderboard
    /// Input an array of placings in acending order of scores
    /// Placings are displayed according to the given array order
    /// </summary>
    /// <param name="placings"></param>
    public void Set(Placing[] placings)
    {
        Array.Clear(currentPlacings, 0, currentPlacings.Length);
        Array.Copy(placings, currentPlacings, 10);

        for (int i = 0; i < listingRowCount; ++i)
        {
            Transform row = placingList.GetChild(i);
            if (i < placings.Length)
            {
                Placing pEntry = placings[i];
                row.gameObject.SetActive(true);
                SetRowDisplayData(row, pEntry);
            }
            else
            {
                row.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Set the leaderboard from currentPlacings to newPlacings
    /// Placings are displayed according to order in newPlacings array
    /// </summary>
    /// <param name="newplacings"></param>
    public void SetAnimated(Placing[] newplacings)
    {
        Array.Clear(newPlacings, 0, newPlacings.Length);
        Array.Copy(newplacings, newPlacings, 10);
        Array.Copy(newplacings, currentPlacings, 10);

        //Change row info to their current position
        for (int i = 0; i < newPlacings.Length && i < placingList.childCount; ++i)
        {
            int newPlacing = i;
            Placing entry = newPlacings[i];
            Transform row = placingList.GetChild(i);

            SetRowDisplayData(row, entry);
        }

        AnimatePlacings();
    }

    void AnimatePlacings()
    {
        StartCoroutine(AnimatePlacingRoutine());
    }

    IEnumerator AnimatePlacingRoutine()
    {
        isAnimating = true;

        //Set target local positions
        for (int i = 0; i < newPlacings.Length && i < placingList.childCount; ++i)
        {
            Placing entry = newPlacings[i];

            Transform moveTransform = placingList.GetChild(i).GetChild(0);
            Vector3 startLocal = placingList.GetChild(i).InverseTransformPoint(placingList.GetChild(entry.Position).GetChild(0).position);
            lerpStartPlacingLocalPositions[i] = startLocal;
        }

        // Shift rows to their 'old' placing position and animate to new position.
        for (int i = 0; i < newPlacings.Length && i < placingList.childCount; ++i)
        {
            Transform row = placingList.GetChild(i);
            Placing entry = newPlacings[i];

            if (entry.Position != i)
            {
                Transform moveTransform = row.GetChild(0);
                moveTransform.transform.localPosition = lerpStartPlacingLocalPositions[i];
            }
        }

        yield return this.TransitionCallback(5, t =>
        {
            for (int i = 0; i < newPlacings.Length && i < placingList.childCount; ++i)
            {
                Transform row = placingList.GetChild(i);
                Placing entry = newPlacings[i];

                if (entry.Position != i)
                {
                    Transform moveTransform = row.GetChild(0);
                    moveTransform.localPosition = Vector3.Lerp(lerpStartPlacingLocalPositions[i], Vector3.zero, Easing.InOutQuad(t));
                }
            }
        });

        isAnimating = false;
    }
}
