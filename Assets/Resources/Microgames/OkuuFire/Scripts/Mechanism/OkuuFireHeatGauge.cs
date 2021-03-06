﻿using UnityEngine;

public class OkuuFireHeatGauge : MonoBehaviour, IOkuuFireMechanism
{
    [Header("Method of victory")]
    public bool heatControl = false;
    public float maxHeatSpeed = 0.5F;

    [Header("How quickly you win once in the safe zone")]
    public float stabilizeSpeed = 1;
    [HideInInspector]
    public float destabilizeSpeed = 1;
    
    [Header("Target zone drifting (increases difficulty a lot)")]
    public float driftRange = 0;
    public float driftSpeed = 1;
    
    [Header("Stage settings")]
    public Transform start;
    public float height;

    public Transform indicator;
    public SpriteRenderer targetZone;

    public GameObject[] victoryObjects;

    private float heatSpeed;
    private float heatLevel;
    private float targetStartLevel;
    private float targetLevel;
    private float stability;
    private bool victory;

	void Start ()
    {
        // Set start temperature
        if (heatControl)
        {
            float startLevel = 0.1F + UnityEngine.Random.Range(0F, 0.4F);
            this.SetLevel(startLevel);
            this.heatSpeed = this.maxHeatSpeed;
        }
        else
        {
            this.SetLevel(0);
        }

        // Randomly determine a target temperature
        float targetLevel = 0.4F + UnityEngine.Random.Range(0F, 0.4F);
        this.SetTarget(targetLevel);
        this.targetStartLevel = targetLevel;
	}
	
	void Update ()
    {
        if (!this.victory)
        {
            if (stability >= 1)
                this.DoVictory();
            else
            {
                if (this.heatControl)
                {
                    // Move the heat indicator based on time and heat speed
                    float newHeat = this.heatLevel + (Time.deltaTime * this.heatSpeed);
                    this.SetLevel(newHeat);
                }

                // Drift the target zone if necessary
                float driftAmount =  this.targetLevel - this.targetStartLevel;
                if (Mathf.Abs(driftAmount) >= Mathf.Abs(this.driftRange))
                    this.driftRange = -this.driftRange;
                float newTarget = this.targetLevel + (Time.deltaTime * this.driftSpeed * this.driftRange);
                //this.SetTarget(newTarget);

                // Stabilize when in target zone
                if (this.InTargetZone())
                    stability += Time.deltaTime * this.stabilizeSpeed;
                else if (stability > 0)
                    stability -= Time.deltaTime * this.destabilizeSpeed;
                else
                    stability = 0;
            }
        }
    }

    public void Move(float completion)
    {
        if (this.heatControl)
        {
            float heatRange = this.maxHeatSpeed * 1.75F;

            // Adjust heating speed based on completion amount
            this.heatSpeed = this.maxHeatSpeed - (heatRange * completion);
        }
        else
        {
            this.SetLevel(completion);
        }
    }

    float GetGaugePositionY(float level)
    {
        float gaugeY = this.height * level;
        Vector3 gaugeStart = start.localPosition;
        gaugeY = gaugeStart.y + gaugeY;
        
        return gaugeY;
    }

    void SetLevel(float level)
    {
        if (level > 1)
            level = 1;

        Vector3 newGuagePosition = this.indicator.localPosition;
        newGuagePosition.y = GetGaugePositionY(level);

        this.indicator.localPosition = newGuagePosition;
        this.heatLevel = level;
    }

    void SetTarget(float level)
    {
        Vector3 newGuagePosition = this.targetZone.transform.localPosition;
        newGuagePosition.y = GetGaugePositionY(level);

        this.targetZone.transform.localPosition = newGuagePosition;
        SineWave wave = targetZone.GetComponent<SineWave>();
        if (wave != null)
        {
            wave.resetStartPosition();
            wave.yOffset = Random.Range(0f, 1f);
        }
        this.targetLevel = level;
    }

    bool InTargetZone()
    {
        Vector3 currentPosition = this.targetZone.transform.position;
        currentPosition.y = this.indicator.position.y;
        bool inZone = targetZone.GetComponent<Collider2D>().bounds.Contains(currentPosition);
        
        return inZone;
    }

    void DoVictory()
    {
        MicrogameController.instance.setVictory(true, true);
        this.victory = true;

        foreach (GameObject victoryObject in this.victoryObjects)
        {
            victoryObject.SendMessage("Victory");
        }
        SineWave wave = targetZone.GetComponent<SineWave>();
        if (wave != null)
            wave.enabled = false;
    }
}
