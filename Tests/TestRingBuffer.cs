using NUnit.Framework;

namespace GameCore.Tests {
    public class TestRingBuffer {
        [Test]
        public void TestCapacity() {
            var rb = new RingBuffer<int>(2);
            Assert.IsTrue(rb.Capacity == 2);
        }

        [Test]
        public void TestLength() {
            var rb = new RingBuffer<int>(2);
            Assert.AreEqual(0, rb.Length);

            rb.Enqueue(1);
            Assert.AreEqual(1, rb.Length);

            rb.Enqueue(2);
            Assert.AreEqual(2, rb.Length);

            rb.Dequeue();
            Assert.AreEqual(1, rb.Length);

            rb.Dequeue();
            Assert.AreEqual(0, rb.Length);
        }

        [Test]
        public void TestDequeue() {
            var rb = new RingBuffer<int>(2);

            rb.Enqueue(1);
            rb.Enqueue(2);
            Assert.AreEqual(1, rb.Dequeue());
            Assert.AreEqual(2, rb.Dequeue());
        }

        [Test]
        public void TestToArray() {
            var rb = new RingBuffer<int>(3);
            Assert.AreEqual(new int[] { }, rb.ToArray());

            rb.Enqueue(1);
            rb.Enqueue(2);
            rb.Enqueue(3);
            Assert.AreEqual(new int[] { 1, 2, 3 }, rb.ToArray());

            rb.Dequeue();
            Assert.AreEqual(new int[] { 2, 3 }, rb.ToArray());

            rb.Enqueue(4);
            Assert.AreEqual(new int[] { 2, 3, 4 }, rb.ToArray());
        }

        [Test]
        public void TestIsFull() {
            var rb = new RingBuffer<int>(2);
            Assert.IsTrue(!rb.IsFull);

            rb.Enqueue(1);
            Assert.IsTrue(!rb.IsFull);

            rb.Enqueue(2);
            Assert.IsTrue(rb.IsFull);

            rb.Dequeue();
            Assert.IsTrue(!rb.IsFull);
        }
    }
}
