﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    private readonly List<GameObject> _unitPrefabs = new List<GameObject>();
    public float AttackSpeed;
    public float AttackTolerance;
    private float _circlePiece;
    private float _currentRadius;
    private float _lengthOfStep;
    private float _nextStep;
    public GameObject Player;
    private float _radiansEachUnit;
    public float Radius = 6.0f;

    [HideInInspector] public int Step;

    private bool _stepping;
    public int Steps = 4;
    public float StepSpeed;
    public float TimeBetweenSteps;
    public float Tolerance = 0.02f;
    public GameObject UnitPrefab;
    public List<float> UnitsScale = new List<float>();

    private void Start()
    {
        _lengthOfStep = Radius/Steps;
        _nextStep = TimeBetweenSteps;
        _currentRadius = Radius;
        float radians = 0;
        _radiansEachUnit = (2*Mathf.PI)/UnitsScale.Count;
        CalculateCirclePiece();

        for (var i = 0; i < UnitsScale.Count; i++)
        {
            var unit = Instantiate(UnitPrefab);

            unit.transform.position = UnitPosition(i);
            unit.transform.Rotate(new Vector3(0, 0, (Mathf.Rad2Deg*radians)));

            _unitPrefabs.Add(unit);

            radians += _radiansEachUnit;
        }
    }

    private void Update()
    {
        if (Time.time > _nextStep)
        {
            _stepping = true;
            Step++;
            _nextStep += TimeBetweenSteps;
        }

        if (_stepping)
        {
            _currentRadius -= (StepSpeed*Time.deltaTime);
            CalculateCirclePiece();
        }

        for (var i = 0; i < _unitPrefabs.Count; i++)
        {
            if (Vector3.Distance(UnitPosition(i), Player.transform.position) < _circlePiece + AttackTolerance)
            {
                var travel = (Vector3.Distance(UnitPosition(i), _unitPrefabs[i].transform.position) +
                              (AttackSpeed*Time.deltaTime))/Vector3.Distance(UnitPosition(i), Player.transform.position);

                _unitPrefabs[i].transform.position = Vector3.Lerp(UnitPosition(i), Player.transform.position, travel);
            }
            else
            {
                if (_stepping)
                {
                    _unitPrefabs[i].transform.position = UnitPosition(i);
                }
                else if (Vector3.Distance(UnitPosition(i), _unitPrefabs[i].transform.position) > Tolerance)
                {
                    _unitPrefabs[i].transform.position += (UnitPosition(i) - _unitPrefabs[i].transform.position)*
                                                         AttackSpeed*Time.deltaTime;
                }
            }
        }

        if (_currentRadius <= Radius - (_lengthOfStep*Step))
        {
            _stepping = false;
        }

        if (Step >= Steps)
        {
            Invoke("EndGame", 0.1f);
        }
    }

    private void EndGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    private Vector3 UnitPosition(int index)
    {
        var radians = _radiansEachUnit*index;

        var x = Mathf.Cos(radians)*_currentRadius;
        var y = Mathf.Sin(radians)*_currentRadius;

        return new Vector3(x, y, 0);
    }

    private void CalculateCirclePiece()
    {
        _circlePiece = ((_currentRadius*2)*Mathf.PI)/(360/(Mathf.Rad2Deg*_radiansEachUnit));
    }
}