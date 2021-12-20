
using System.Collections;
using System.Collections.Generic;

public sealed class BitEnumerable : IEnumerable<int> {
    private readonly string hex;

    public BitEnumerable(string hex) {
        this.hex = hex;
    }

    public IEnumerator<int> GetEnumerator() {
        for (int i = 0; i < hex.Length; i++) {
            for (int j = 3; j >= 0; j--) {
                yield return 0x01 & (Convert.ToInt32(hex.Substring(i, 1), 16) >> j);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}