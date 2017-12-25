using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReaderWriterLockSlimExtend
{
    public abstract class Striped<L>
    {
        private Striped() { }

        public abstract L get(Object key);

        public abstract L getAt(int index);

        public abstract int size();

        public abstract int indexFor(Object key);

        private static int smear(int hashcode)
        {
            hashcode ^= MoveByte(hashcode, 20) ^ MoveByte(hashcode, 12);
            return hashcode ^ MoveByte(hashcode, 7) ^ MoveByte(hashcode, 4);
        }

        public static int MoveByte(int value, int pos)
        {
            if (value < 0)
            {
                string s = Convert.ToString(value, 2); // 转换为二进制
                for (int i = 0; i < pos; i++)
                {
                    s = "0" + s.Substring(0, 31);
                }
                return Convert.ToInt32(s, 2); // 将二进制数字转换为数字
            }
            else
            {
                return value >> pos;
            }
        }


        private static readonly int ALL_SET = ~0;

        private static int ceilToPowerOfTwo(int x)
        {
            return 1 << (int)Math.Ceiling(Math.Log(x, 2));
        }

        public static Striped<ReaderWriterLockSlim> lazyWeakLock(int stripes)
        {
            //Lazy<ReaderWriterLockSlim> rwls = new Lazy<ReaderWriterLockSlim>(
            //    () => { return new ReaderWriterLockSlim(); });
            return new LargeLazyStriped<ReaderWriterLockSlim>(stripes, () => { return new ReaderWriterLockSlim(); });
        }



        class LargeLazyStriped<L> : Striped<L> where L : class
        {
            readonly ConcurrentDictionary<int, WeakReference<L>> locks;
            readonly Func<L> supplier;
            int _size;

            public LargeLazyStriped(int stripes, Func<L> supplier)
            {

                this._size = (mask == ALL_SET) ? int.MaxValue : mask + 1;
                this.supplier = supplier;
                this.locks = new ConcurrentDictionary<int, WeakReference<L>> { };
                this.mask = stripes > 1 << (32 - 2) ? ALL_SET : ceilToPowerOfTwo(stripes) - 1;
            }


            readonly int mask = 0;

            public override L get(object key)
            {
                return getAt(indexFor(key));
            }

            public override int indexFor(object key)
            {
                int hash = smear(key.GetHashCode());
                return hash & mask;
            }

            public override int size()
            {
                return _size;
            }

            public override L getAt(int index)
            {
                WeakReference<L> existing;
                L tl;
                if (locks.TryGetValue(index, out existing))
                {
                    if (existing.TryGetTarget(out tl))
                    {
                        return tl;
                    }
                    else
                    {
                        //locks.AddOrUpdate
                        if (Monitor.TryEnter(existing))
                        {
                            tl = this.supplier();
                            existing.SetTarget(tl);
                            Monitor.Exit(existing);
                            return tl;
                        }
                        else
                        {
                            return getAt(index);
                        }
                        //return getAt(index);
                    }
                }
                else
                {
                    existing = new WeakReference<L>(null);
                    locks.TryAdd(index, existing);
                    return getAt(index);
                }
            }
        }
    }
}
