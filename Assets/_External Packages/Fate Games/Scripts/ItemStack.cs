using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FateGames
{
    public class ItemStack : MonoBehaviour
    {
        [SerializeField] private float itemAdjustingSpeed = 40;
        [SerializeField] private float itemAdjustingRotatingSpeed = 45;
        private List<Stackable> collection = new List<Stackable>();
        private Transform _transform;
        [SerializeField] private Swerve swerve;
        [SerializeField] private float _spring = 0.005f;
        private float spring = 0.00f;
        private AnimationCurve curve = new AnimationCurve();
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Quaternion itemRotation;
        [SerializeField] private bool wave = false;
        [SerializeField] private bool immobile = false;
        [SerializeField] private int limit = -1;
        private UnityEvent<Stackable> onAdd = new UnityEvent<Stackable>();
        private UnityEvent<Stackable> onRemove = new UnityEvent<Stackable>();
        private Tween audioPitchResetTween = null;
        private Tween springTween = null;
        [SerializeField] private float pushPeriod = 0.2f;
        private float lastPushTime = -1f;


        public UnityEvent<Stackable> OnAdd { get => onAdd; }

        public bool CanPush { get => Time.time >= lastPushTime + pushPeriod && ((limit >= 0 && collection.Count < limit) || limit < 0); }
        public int Size { get => collection.Count; }
        public Transform Transform { get => _transform; }

        private void Awake()
        {
            _transform = transform;
            InitializeCurve();
            SetAudioSource();
            SetSwerve();
        }

        #region Swerve
        private void SetSwerve()
        {
            swerve?.OnStart.AddListener(() =>
            {
                if (springTween != null)
                {
                    springTween.Kill();
                    springTween = null;
                }
                spring = _spring;
            });
            swerve?.OnRelease.AddListener(() =>
            {
                springTween = DOTween.To((val) =>
                {
                    spring = val < 0 ? val * 3 : val;
                }, _spring, 0, 1);
            });
        }
        #endregion

        #region Audio
        private void SetAudioSource()
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource) return;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        private void ResetAudioPitch()
        {
            if (audioPitchResetTween != null)
            {
                audioPitchResetTween.Kill();
            }
            audioPitchResetTween = DOVirtual.DelayedCall(pushPeriod + 0.05f, () =>
            {
                audioSource.pitch = 1;
                audioPitchResetTween = null;
            });

        }

        private void PlayAddingAudio()
        {
            if (!audioSource) return;
            audioSource.Play();
            audioSource.pitch += 0.05f;
            ResetAudioPitch();
        }

        #endregion

        #region Curve

        private void InitializeCurve()
        {
            curve.AddKey(0, 0);
            curve.AddKey(0.5f, 1.5f);
            curve.AddKey(1, 0);
        }
        private void SetCurve()
        {
            Keyframe newKey = curve.keys[1];
            newKey.value = 1.5f + collection.Count * 0.2f;
            curve.MoveKey(1, newKey);
        }
        #endregion
        private void FixedUpdate()
        {
            if (immobile) return;
            if (collection.Count == 0) return;
            Stackable firstItem = collection[0];
            firstItem.Transform.position = Vector3.Lerp(firstItem.Transform.position, _transform.position, itemAdjustingSpeed * Time.fixedDeltaTime);
            firstItem.Transform.rotation = Quaternion.Lerp(firstItem.Transform.rotation, _transform.rotation, itemAdjustingRotatingSpeed * Time.fixedDeltaTime);
            for (int i = 1; i < collection.Count; i++)
            {
                Stackable previousItem = collection[i - 1];
                Stackable currentItem = collection[i];
                currentItem.Transform.position = Vector3.Lerp(currentItem.Transform.position, -_transform.forward * spring * i + previousItem.Transform.position + Vector3.up * (previousItem.StackableTag == currentItem.StackableTag ? previousItem.IdenticalMargin : previousItem.DifferentMargin), Time.fixedDeltaTime * itemAdjustingSpeed);
                currentItem.Transform.rotation = Quaternion.Lerp(currentItem.Transform.rotation, previousItem.Transform.rotation, Time.fixedDeltaTime * itemAdjustingRotatingSpeed);
            }
        }

        // Adds the item to the end of the stack.
        public void Push(Stackable newItem, bool audio = false, bool overrideWave = false)
        {
            if (!newItem || !CanPush) return;
            lastPushTime = Time.time;
            if (audio)
                PlayAddingAudio();
            SetCurve();
            Vector3 start = newItem.Transform.position;
            DOTween.To((val) =>
            {
                Vector3 end;
                Stackable previousItem = null;
                if (collection.Count == 0)
                    end = _transform.position;
                else
                {
                    previousItem = collection[collection.Count - 1];
                    end = previousItem.Transform.position + Vector3.up * (previousItem.StackableTag == newItem.StackableTag ? previousItem.IdenticalMargin : previousItem.DifferentMargin);
                }
                Vector3 pos = Vector3.Lerp(start, end, val);
                pos.y += curve.Evaluate(val);
                newItem.Transform.position = pos;
                if (previousItem)
                    newItem.Transform.rotation = Quaternion.Lerp(newItem.Transform.rotation, previousItem.Transform.rotation, Time.fixedDeltaTime * itemAdjustingRotatingSpeed);
                else
                    newItem.Transform.rotation = Quaternion.Lerp(newItem.Transform.rotation, _transform.rotation, Time.fixedDeltaTime * itemAdjustingRotatingSpeed);
            }, 0, 1, pushPeriod - 0.01f).OnComplete(() =>
            {
                collection.Add(newItem);
                onAdd.Invoke(newItem);
                if (overrideWave || wave)
                    StartWave();
            });

        }

        private void StartWave()
        {
            StartCoroutine(_Wave(collection.Count - 1));
        }

        private IEnumerator _Wave(int index)
        {
            if (index < 0 || index >= collection.Count) yield break;
            Stackable item = collection[index];
            if (item.WaveScaleTween != null)
            {
                item.WaveScaleTween.Kill();
                item.Transform.localScale = Vector3.one;
            }
            item.WaveScaleTween = item.Transform.DOScale(1.35f, 0.05f).SetLoops(2, LoopType.Yoyo);
            yield return new WaitForSeconds(0.03f);
            StartCoroutine(_Wave(index - 1));
        }

        // Adds the item to the front of the stack.
        public void Insert(Stackable newItem)
        {
            newItem.Transform.parent = _transform;
            collection.Insert(0, newItem);
            newItem.Transform.localRotation = itemRotation;
        }

        public Stackable Pop()
        {
            return RemoveAt(collection.Count - 1);
        }

        public Stackable Dequeue()
        {
            Stackable result = RemoveAt(0);
            return result;
        }
        private Stackable RemoveAt(int index)
        {
            if (collection.Count == 0)
            {
                //Debug.LogError("Cannot remove from empty stack!", this);
                return null;
            }
            Stackable objectToPop = collection[index];
            collection.RemoveAt(index);
            objectToPop.transform.parent = null;
            onRemove.Invoke(objectToPop);
            return objectToPop;
        }

        public bool Transfer(ItemStack otherStack, bool audio = false, bool overrideWave = false)
        {
            if (!otherStack.CanPush) return false;
            bool result = false;
            Stackable item = Pop();
            if (item)
            {
                otherStack.Push(item, audio, overrideWave);
                result = true;
            }
            return result;
        }
    }

}
