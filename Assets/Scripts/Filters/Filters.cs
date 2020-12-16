using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Filters
{
    namespace FloatFilters
    {
        public class RuningAverageFilter
        {
            private Queue<float> _data;
            private float _sum;
            public float P;

            public RuningAverageFilter(int count)
            {
                _data = new Queue<float>();
                for (int i = 0; i < Mathf.Abs(count); i++)
                {
                    _data.Enqueue(0);
                }
            }

            public void NewValue(float value)
            {
                _data.Enqueue(value);
                _sum += value;
                _sum -= _data.Dequeue();
            }

            public float GetValue()
            {
                return _sum / _data.Count;
            }

            public void SetFilterCapacity(int value)
            {
                var count = value - _data.Count;
                if (count < 0)
                    for (int i = 0; i < Mathf.Abs(count); i++)
                    {
                        _data.Dequeue();
                    }
                else
                    for (int i = 0; i < count; i++)
                    {
                        _data.Enqueue(0);
                    }
            }

            public int GetFilterCapacity()
            {
                return _data.Count;
            }
        }

        public class AverageFilter
        {
            public static float GetAverage(float[] data)
            {
                float sum = 0;
                foreach (float item in data)
                {
                    sum += item;
                }
                return sum / data.Length;
            }
        }

        public class ThrasholdFilter
        {
            public float Thrashold;

            private float _prevValue;

            public ThrasholdFilter(float thrashold)
            {
                Thrashold = thrashold;
            }

            public void NewValue(float value)
            {
                if (System.Math.Abs(value - _prevValue) > Thrashold) _prevValue = value;
            }

            public float GetValue()
            {
                return _prevValue;
            }
        }

        public class ThrasholdRuningAverageFilter
        {
            public float Thrashold;

            private RuningAverageFilter _runingAverage;
            private float _prevValue;
            private float _newValue;

            public ThrasholdRuningAverageFilter(float thrashold, int count)
            {
                Thrashold = thrashold;
                _runingAverage = new RuningAverageFilter(count);
            }

            public void NewValue(float value)
            {
                _runingAverage.NewValue(value);
                if (System.Math.Abs(value - _prevValue) > Thrashold) _prevValue = value;
                _newValue = value;
            }

            public float GetValue()
            {
                return (System.Math.Abs(_newValue - _prevValue) > Thrashold) ? _newValue : _runingAverage.GetValue();
            }

            public void SetFilterCapacity(int value)
            {
                _runingAverage.SetFilterCapacity(value);
            }

            public int GetFilterCapacity()
            {
                return _runingAverage.GetFilterCapacity();
            }
        }

    }

    namespace Vector3Filters
    {
        public class RuningAverageFilter
        {
            private Queue<Vector3> _data;
            private Vector3 _sum;
            public Vector3 P;

            public RuningAverageFilter(int count)
            {
                _data = new Queue<Vector3>();
                for (int i = 0; i < Mathf.Abs(count); i++)
                {
                    _data.Enqueue(Vector3.zero);
                }
            }

            public void NewValue(Vector3 value)
            {
                _data.Enqueue(value);
                _sum += value;
                _sum -= _data.Dequeue();
            }

            public Vector3 GetValue()
            {
                return _sum / _data.Count;
            }

            public void SetFilterCapacity(int value)
            {
                var count = value - _data.Count;
                if (count < 0)
                    for (int i = 0; i < Mathf.Abs(count); i++)
                    {
                        _data.Dequeue();
                    }
                else
                    for (int i = 0; i < count; i++)
                    {
                        _data.Enqueue(Vector3.zero);
                    }
            }

            public int GetFilterCapacity()
            {
                return _data.Count;
            }
        }

        public class AverageFilter
        {
            public static Vector3 GetAverage(Vector3[] data)
            {
                Vector3 sum = Vector3.zero;
                foreach (Vector3 item in data)
                {
                    sum += item;
                }
                return sum / data.Length;
            }
        }

        public class ThrasholdFilter
        {
            public float Thrashold;

            private Vector3 _prevValue;

            public ThrasholdFilter(float thrashold)
            {
                Thrashold = thrashold;
            }

            public void NewValue(Vector3 value)
            {
                if (System.Math.Abs(value.x - _prevValue.x) > Thrashold) _prevValue.x = value.x;
                if (System.Math.Abs(value.y - _prevValue.y) > Thrashold) _prevValue.y = value.y;
                if (System.Math.Abs(value.z - _prevValue.z) > Thrashold) _prevValue.z = value.z;
            }

            public Vector3 GetValue()
            {
                return _prevValue;
            }
        }

        public class ThrasholdRuningAverageFilter
        {
            public float Thrashold;

            private RuningAverageFilter _runingAverage;
            private Vector3 _prevValue;
            private Vector3 _newValue;

            public ThrasholdRuningAverageFilter(float thrashold, int count)
            {
                Thrashold = thrashold;
                _runingAverage = new RuningAverageFilter(count);
            }

            public void NewValue(Vector3 value)
            {
                _runingAverage.NewValue(value);
                if (System.Math.Abs(value.x - _prevValue.x) > Thrashold) _prevValue.x = value.x;
                if (System.Math.Abs(value.y - _prevValue.y) > Thrashold) _prevValue.y = value.y;
                if (System.Math.Abs(value.z - _prevValue.z) > Thrashold) _prevValue.z = value.z;
                _newValue = value;
            }

            public Vector3 GetValue()
            {
                Vector3 vector = new Vector3();
                vector.x = (System.Math.Abs(_newValue.x - _prevValue.x) > Thrashold) ? _newValue.x : _runingAverage.GetValue().x;
                vector.y = (System.Math.Abs(_newValue.y - _prevValue.y) > Thrashold) ? _newValue.y : _runingAverage.GetValue().y;
                vector.z = (System.Math.Abs(_newValue.z - _prevValue.z) > Thrashold) ? _newValue.z : _runingAverage.GetValue().z;

                return vector;
            }

            public void SetFilterCapacity(int value)
            {
                _runingAverage.SetFilterCapacity(value);
            }

            public int GetFilterCapacity()
            {
                return _runingAverage.GetFilterCapacity();
            }
        }
    }

    namespace Vector4Filters
    {
        public class RuningAverageFilter
        {
            private Queue<Vector4> _data;
            private Vector4 _sum;
            public Vector4 P;

            public RuningAverageFilter(int count)
            {
                _data = new Queue<Vector4>();
                for (int i = 0; i < Mathf.Abs(count); i++)
                {
                    _data.Enqueue(Vector4.zero);
                }
            }

            public void NewValue(Vector4 value)
            {
                _data.Enqueue(value);
                _sum += value;
                _sum -= _data.Dequeue();
            }

            public Vector4 GetValue()
            {
                return _sum / _data.Count;
            }

            public void SetFilterCapacity(int value)
            {
                var count = value - _data.Count;
                if (count < 0)
                    for (int i = 0; i < Mathf.Abs(count); i++)
                    {
                        _data.Dequeue();
                    }
                else
                    for (int i = 0; i < count; i++)
                    {
                        _data.Enqueue(Vector4.zero);
                    }
            }

            public int GetFilterCapacity()
            {
                return _data.Count;
            }
        }

        public class AverageFilter
        {
            public static Vector4 GetAverage(Vector4[] data)
            {
                Vector4 sum = Vector4.zero;
                foreach (Vector4 item in data)
                {
                    sum += item;
                }
                return sum / data.Length;
            }
        }

        public class ThrasholdFilter
        {
            public float Thrashold;

            private Vector4 _prevValue;

            public ThrasholdFilter(float thrashold)
            {
                Thrashold = thrashold;
            }

            public void NewValue(Vector4 value)
            {
                if (System.Math.Abs(value.x - _prevValue.x) > Thrashold) _prevValue.x = value.x;
                if (System.Math.Abs(value.y - _prevValue.y) > Thrashold) _prevValue.y = value.y;
                if (System.Math.Abs(value.z - _prevValue.z) > Thrashold) _prevValue.z = value.z;
                if (System.Math.Abs(value.w - _prevValue.w) > Thrashold) _prevValue.w = value.w;
            }

            public Vector4 GetValue()
            {
                return _prevValue;
            }
        }

        public class ThrasholdRuningAverageFilter
        {
            public float Thrashold;

            private RuningAverageFilter _runingAverage;
            private Vector4 _prevValue;
            private Vector4 _newValue;

            public ThrasholdRuningAverageFilter(float thrashold, int count)
            {
                Thrashold = thrashold;
                _runingAverage = new RuningAverageFilter(count);
            }

            public void NewValue(Vector4 value)
            {
                _runingAverage.NewValue(value);
                if (System.Math.Abs(value.x - _prevValue.x) > Thrashold) _prevValue.x = value.x;
                if (System.Math.Abs(value.y - _prevValue.y) > Thrashold) _prevValue.y = value.y;
                if (System.Math.Abs(value.z - _prevValue.z) > Thrashold) _prevValue.z = value.z;
                if (System.Math.Abs(value.w - _prevValue.w) > Thrashold) _prevValue.w = value.w;
                _newValue = value;
            }

            public Vector4 GetValue()
            {
                Vector4 vector = new Vector4();
                vector.x = (System.Math.Abs(_newValue.x - _prevValue.x) > Thrashold) ? _newValue.x : _runingAverage.GetValue().x;
                vector.y = (System.Math.Abs(_newValue.y - _prevValue.y) > Thrashold) ? _newValue.y : _runingAverage.GetValue().y;
                vector.z = (System.Math.Abs(_newValue.z - _prevValue.z) > Thrashold) ? _newValue.z : _runingAverage.GetValue().z;
                vector.w = (System.Math.Abs(_newValue.w - _prevValue.w) > Thrashold) ? _newValue.w : _runingAverage.GetValue().w;

                return vector;
            }

            public void SetFilterCapacity(int value)
            {
                _runingAverage.SetFilterCapacity(value);
            }

            public int GetFilterCapacity()
            {
                return _runingAverage.GetFilterCapacity();
            }
        }

        public class ThrasholdFilterL2
        {
            public float Thrashold;

            private Vector4 _prevValue;
            private bool[] next = new bool[4];

            public ThrasholdFilterL2(float thrashold)
            {
                Thrashold = thrashold;
            }

            public void NewValue(Vector4 value)
            {
                NewValueAlg(0, ref _prevValue.x, ref value.x);
                NewValueAlg(1, ref _prevValue.y, ref value.y);
                NewValueAlg(2, ref _prevValue.z, ref value.z);
                NewValueAlg(3, ref _prevValue.w, ref value.w);
            }

            private void NewValueAlg(int index, ref float prevVal, ref float newVal)
            {
                if (next[index])
                {
                    if (!(System.Math.Abs(newVal - prevVal) > Thrashold)) next[index] = false;
                    prevVal = newVal;
                }
                else
                {
                    if (System.Math.Abs(newVal - prevVal) > Thrashold)
                    {
                        prevVal = newVal;
                        next[index] = true;
                    }
                }
            }

            public Vector4 GetValue()
            {
                return _prevValue;
            }
        }

    }

    public enum Filters
    {
        None,
        RuningAverageFilter,
        ThrasholdFilter,
        ThrasholdRuningAverageFilter
    }


}
