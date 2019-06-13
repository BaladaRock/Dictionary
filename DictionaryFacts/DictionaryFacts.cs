using System;
using Xunit;
using Dictionary;
using System.Collections.Generic;

namespace DictionaryFacts
{
    public class DictionaryFacts
    {
        [Fact]
        public void Test_Enumerator_Should_Return_False_For_Empty_Dictionary()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>();
            //When
            var enumerator = dictionary.GetEnumerator();
            //Then
            Assert.False(enumerator.MoveNext());
            Assert.Empty(dictionary);
        }

        [Fact]
        public void Test_Enumerator_Should_Properly_Work_When_Dictionary_has_1_element()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>();
            var enumerator = dictionary.GetEnumerator();
            //When
            dictionary.Add(1, 1);
            enumerator.MoveNext();
            //Then
            Assert.Equal(1, enumerator.Current.Key);
            Assert.Equal(1, enumerator.Current.Value);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void Test_Enumerator_Should_Properly_Work_When_Dictionary_has_MORE_elements()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>();
            var enumerator = dictionary.GetEnumerator();
            //When
            dictionary.Add(1, 1);
            dictionary.Add(2, 1);
            dictionary.Add(3, 100);
            enumerator.MoveNext();
            //Then
            Assert.Equal(1, enumerator.Current.Key);
            Assert.Equal(1, enumerator.Current.Value);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current.Key);
            Assert.Equal(1, enumerator.Current.Value);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(3, enumerator.Current.Key);
            Assert.Equal(100, enumerator.Current.Value);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void Test_Enumerator_Should_Properly_Work_Same_HashCode_Is_Used_For_More_Elements()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>();
            var enumerator = dictionary.GetEnumerator();
            //When
            dictionary.Add(1, 1);
            dictionary.Add(6, 2);
            dictionary.Add(3, 100);
            dictionary.Add(8, 3);
            enumerator.MoveNext();
            //Then
            Assert.Equal(new KeyValuePair<int, int>(1, 1), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<int, int>(6, 2), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<int, int>(3, 100), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<int, int>(8, 3), enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void Test_AddMethod_Should_Correctly_Add_Int_Key_And_Value_No_Conflict()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>
            {
                { 11, 2 }
            };
            //When
            var enumerator = dictionary.GetEnumerator();
            //Then
            Assert.Single(dictionary);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<int, int>(11, 2), enumerator.Current);
        }

        [Fact]
        public void Test_AddMethod_Should_Correctly_Add_Same_Hash_Code()
        {
            //Given
            var dictionary = new HashtableDictionary<int, string>();
            //When
            dictionary.Add(11, "1");
            dictionary.Add(6, "1");
            dictionary.Add(1, "Andrei");
            var enumerator = dictionary.GetEnumerator();
            //Then
            Assert.Equal(3, dictionary.Count);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<int, string>(11, "1"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<int, string>(6, "1"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<int, string>(1, "Andrei"), enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void Test_AddMethod_Should_Work_Properly_Second_Overload()
        {
            //Given
            var dictionary = new HashtableDictionary<int, string>();
            //When
            dictionary.Add(11, "1");
            dictionary.Add(new KeyValuePair<int, string>(1, "Andrei"));
            var enumerator = dictionary.GetEnumerator();
            //Then
            Assert.Equal(2, dictionary.Count);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<int, string>(11, "1"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<int, string>(1, "Andrei"), enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void Test_Exception_AddMethod_Should_Not_Add_Same_Key_Twice()
        {
            //Given
            var dictionary = new HashtableDictionary<int, string>
            {
                { 11, "1" }
            };
            //When
            var exception = Assert.Throws<ArgumentException>(() => dictionary.Add(11, "Andrei"));
            var enumerator = dictionary.GetEnumerator();
            //Then
            Assert.Single(dictionary);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<int, string>(11, "1"), enumerator.Current);
            Assert.False(enumerator.MoveNext());
            Assert.Equal("Key 11 already exists in dictionary!", exception.Message);
        }

        [Fact]
        public void Test_Exception_AddMethod_Should_Not_Add_When_List_is_ReadOnly()
        {
            //Given
            var dictionary = new Dictionary.HashtableDictionary<int, string>
            {
                { 11, "1" }
            };
            var ro = dictionary.AsReadOnly();
            //When
            var exception = Assert.Throws<NotSupportedException>(() => ro.Add(2, "Andrei"));
            //Then
            Assert.Single(dictionary);
            Assert.Single(ro);
            Assert.True(ro.IsReadOnly);
            Assert.False(dictionary.IsReadOnly);
            Assert.Equal("List is readonly!\n", exception.Message);
        }

        [Fact]
        public void Test_KeysProperty_Should_Correctly_Return_List_Of_Keys()
        {
            //Given
            var dictionary = new HashtableDictionary<string, string>();
            //When
            dictionary.Add("Andrei", "Andrei");
            dictionary.Add("Eusebiu", string.Empty);
            dictionary.Add("1", "2");
            //Then
            Assert.Equal(new List<string> { "Andrei", "Eusebiu", "1" }, dictionary.Keys);
        }

        [Fact]
        public void Test_KeysProperty_Should_Correctly_Return_List_Of_Keys_For_Empty_Dictionary()
        {
            //When
            var dictionary = new HashtableDictionary<string, string>();
            //Then
            Assert.Equal(new List<string>(), dictionary.Keys);
        }

        [Fact]
        public void Test_ValuesProperty_Should_Correctly_Return_List_Of_Keys()
        {
            //Given
            var dictionary = new HashtableDictionary<string, string>();
            //When
            dictionary.Add("Andrei", "Andrei");
            dictionary.Add("Eusebiu", string.Empty);
            dictionary.Add("1", "2");
            //Then
            Assert.Equal(new List<string> { "Andrei", string.Empty, "2" }, dictionary.Values);
        }

        [Fact]
        public void Test_ValuesProperty_Should_Correctly_Return_List_Of_Keys_For_Empty_Dictionary()
        {
            //When
            var dictionary = new HashtableDictionary<string, string>();
            //Then
            Assert.Equal(new List<string>(), dictionary.Values);
        }

        [Fact]
        public void Test_ContainsKey_Method_Should_Return_True_1_element_Dictionary()
        {
            //Given
            var dictionary = new HashtableDictionary<string, string>
            {
                //When
                { "Andrei", "1" }
            };
            //Then
            Assert.Single(dictionary);
            Assert.True(dictionary.ContainsKey("Andrei"));
        }

        [Fact]
        public void Test_Indexer_Get_For_Simple_Case()
        {
            //When
            var dictionary = new HashtableDictionary<string, string>
            {
                { "Andrei", "1" }
            };
            //Then
            Assert.Equal("1", dictionary["Andrei"]);
        }

        [Fact]
        public void Test_Indexer_Set_For_A_Simple_Case()
        {
            //Given
            var dictionary = new HashtableDictionary<string, string>
            {
                { "Andrei", "1" }
            };
            //When
            dictionary["Andrei"] = "Eusebiu";
            //Then
            Assert.Equal("Eusebiu", dictionary["Andrei"]);
            Assert.Single(dictionary);
        }

        [Fact]
        public void Test_ContainsKey_Method_ShouldWork_Properly_More_When_Elements_have_CONFLICTS()
        {
            //Given
            var dictionary = new HashtableDictionary<int, string>();
            //When
            dictionary.Add(1, "Andrei");
            dictionary.Add(6, "Eusebiu");
            dictionary.Add(11, "Andrei");
            dictionary.Add(16, "GTA");
            //Then
            Assert.Equal(4, dictionary.Count);
            Assert.True(dictionary.ContainsKey(1));
            Assert.True(dictionary.ContainsKey(6));
            Assert.True(dictionary.ContainsKey(11));
            Assert.True(dictionary.ContainsKey(16));
            Assert.False(dictionary.ContainsKey(21));
            Assert.False(dictionary.ContainsKey(7));
            Assert.False(dictionary.ContainsKey(0));
        }

        [Fact]
        public void Test_Contains_Method_ShouldWork_Properly_More_When_Elements_have_CONFLICTS()
        {
            //Given
            var dictionary = new HashtableDictionary<int, string>();
            //When
            dictionary.Add(1, "Andrei");
            dictionary.Add(6, "Eusebiu");
            dictionary.Add(11, "Andrei");
            dictionary.Add(16, "GTA");
            //Then
            Assert.Equal(4, dictionary.Count);
            Assert.True(dictionary.Contains(new KeyValuePair<int, string>(1, "Andrei")));
            Assert.True(dictionary.Contains(new KeyValuePair<int, string>(6, "Eusebiu")));
            Assert.True(dictionary.Contains(new KeyValuePair<int, string>(11, "Andrei")));
            Assert.True(dictionary.Contains(new KeyValuePair<int, string>(16, "GTA")));
            Assert.False(dictionary.Contains(new KeyValuePair<int, string>(3, "Ciocolata")));
            Assert.False(dictionary.Contains(new KeyValuePair<int, string>(100, "Andrei")));
            Assert.False(dictionary.Contains(new KeyValuePair<int, string>(1, "Ciocolata")));
        }

        [Fact]
        public void Test_TryGetValue_Method_Should_Return_True_When_Key_Is_Found()
        {
            //When
            var dictionary = new HashtableDictionary<int, string>
            {
                { 1, "Andrei" },
                { 6, "Eusebiu" },
                { 11, "Andrei" },
                { 16, "GTA" }
            };
            //Then
            Assert.True(dictionary.TryGetValue(1, out string value));
        }

        [Fact]
        public void Test_TryGetValue_Method_Should_Return_False_When_Key_Does_Not_Exist()
        {
            //When
            var dictionary = new HashtableDictionary<int, string>
            {
                { 1, "Andrei" },
                { 6, "Eusebiu" },
                { 11, "Andrei" },
                { 16, "GTA" }
            };
            //Then
            Assert.False(dictionary.TryGetValue(100, out string value));
        }

        [Fact]
        public void Test_Exception_GetIndexer_Should_Throw_Exception_When_Key_Is_Null()
        {
            //Given
            var dictionary = new HashtableDictionary<string, string>
            {
                { "11", "1"}
            };
            //When
            var exception = Assert.Throws<ArgumentNullException>(() => dictionary[null]);
            //Then
            Assert.Equal("key", exception.ParamName);
        }

        [Fact]
        public void Test_Exception_GetIndexer_Should_Throw_Exception_When_Key_Is_NOT_found()
        {
            //Given
            var dictionary = new HashtableDictionary<string, string>
            {
                { "11", "1"}
            };
            //When
            var exception = Assert.Throws<KeyNotFoundException>(() => dictionary["100"]);
            //Then
            Assert.Equal("Key 100 was not found! ", exception.Message);
        }

        [Fact]
        public void Test_Exception_SetIndexer_Should_Throw_Exception_When_Key_Is_NOT_found()
        {
            //Given
            var dictionary = new HashtableDictionary<string, string>
            {
                { "11", "1"}
            };
            //When
            var exception = Assert.Throws<KeyNotFoundException>(() => dictionary["100"] = "300");
            //Then
            Assert.Equal("Key 100 was not found! ", exception.Message);
        }

        [Fact]
        public void Test_CopyMethod_Array_Has_Enough_Capacity()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>();
            KeyValuePair<int, int>[] array = new KeyValuePair<int, int>[6];
            //When
            dictionary.Add(11, 1);
            dictionary.Add(2, 3);
            dictionary.Add(3, 4);
            dictionary.CopyTo(array, 2);
            //Then
            Assert.Equal(3, dictionary.Count);
            Assert.NotEqual(1, array[5].Value);
            Assert.Equal(3, array[4].Key);
            Assert.Equal(4, array[4].Value);
            Assert.NotEqual(11, array[0].Key);
            Assert.NotEqual(4, array[5].Value);
        }

        [Fact]
        public void Test_Exception_CopyMethod_Array_Does_Not_Have_Enough_Capacity()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>();
            KeyValuePair<int, int>[] array = new KeyValuePair<int, int>[3];
            //When
            dictionary.Add(11, 1);
            dictionary.Add(2, 3);
            dictionary.Add(3, 4);
            var exception = Assert.Throws<ArgumentException>(() => dictionary.CopyTo(array, 2));
            //Then
            Assert.Equal(3, dictionary.Count);
            Assert.Equal(3, array.Length);
            Assert.Equal("array", exception.ParamName);
        }

        [Fact]
        public void Test_Exception_CopyMethod_Array_Index_Is_Smaller_Then_Zero()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>();
            KeyValuePair<int, int>[] array = new KeyValuePair<int, int>[6];
            //When
            dictionary.Add(11, 1);
            dictionary.Add(2, 3);
            dictionary.Add(3, 4);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.CopyTo(array, -3));
            //Then
            Assert.Equal(3, dictionary.Count);
            Assert.Equal(6, array.Length);
            Assert.Equal("arrayIndex", exception.ParamName);
        }

        [Fact]
        public void Test_Exception_CopyMethod_Array_Is_Null()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>();
            KeyValuePair<int, int>[] array = null;
            //When
            dictionary.Add(11, 1);
            dictionary.Add(2, 3);
            dictionary.Add(3, 4);
            var exception = Assert.Throws<ArgumentNullException>(() => dictionary.CopyTo(array, 2));
            //Then
            Assert.Equal(3, dictionary.Count);
            Assert.Equal("array", exception.ParamName);
        }

        [Fact]
        public void Test_RemoveMethod_Should_Correctly_Remove_Element_No_Conflict()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>(bucketSize: 2)
            {
                { 10, 2 },
                {1, 5 },
            };
            //Then
            Assert.True(dictionary.Remove(1));
            Assert.Single(dictionary);
            Assert.False(dictionary.TryGetValue(1, out int value));
        }

        [Fact]
        public void Test_RemoveMethod_Should_Correctly_Remove_More_Elements_No_Conflict()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>()
            {
                { 0, 100},
                {1, 100 },
                {2, 100 },
                {3, 100 },
                {4, 100 }
            };
            //Then
            Assert.True(dictionary.Remove(0));
            Assert.True(dictionary.Remove(1));
            Assert.True(dictionary.Remove(2));
            Assert.Equal(2, dictionary.Count);
            Assert.False(dictionary.TryGetValue(0, out int firstValue));
            Assert.False(dictionary.TryGetValue(1, out int secondValue));
            Assert.False(dictionary.ContainsKey(2));
        }

        [Fact]
        public void Test_RemoveMethod_Should_Correctly_Work_For_Conflict()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>()
            {
                { 0, 100},
                {1, 100 },
                {11, 100 },
                {5, 100 },
                {16, 100 }
            };
            // When
            dictionary.Remove(16);
            //Then
            Assert.False(dictionary.ContainsKey(16));
            Assert.True(dictionary.ContainsKey(11));
            Assert.True(dictionary.ContainsKey(1));
        }

        [Fact]
        public void Test_RemoveMethod_Conflict_Element_to_be_removed_Is_In_The_Middle()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>()
            {
                { 0, 100},
                {1, 100 },
                {11, 100 },
                {5, 100 },
                {16, 100 }
            };
            //When
            dictionary.Remove(11);
            //Then
            Assert.True(dictionary.ContainsKey(0));
            Assert.True(dictionary.ContainsKey(5));
            Assert.True(dictionary.ContainsKey(16));
            Assert.True(dictionary.ContainsKey(1));
            Assert.False(dictionary.ContainsKey(11));
        }

        [Fact]
        public void Test_RemoveMethod_Second_Overload()
        {
            //Given
            var pair = new KeyValuePair<int, int>(11, 100);
            var dictionary = new HashtableDictionary<int, int>()
            {
                { 0, 100},
                {1, 100 },
                pair,
                {5, 100 },
                {16, 100 }
            };
            //When
            dictionary.Remove(pair);
            //Then
            Assert.True(dictionary.ContainsKey(0));
            Assert.True(dictionary.ContainsKey(5));
            Assert.True(dictionary.ContainsKey(16));
            Assert.True(dictionary.ContainsKey(1));
            Assert.False(dictionary.ContainsKey(11));
        }

        [Fact]
        public void Test_AddMethod_Should_Resize_When_Dictionary_Is_full()
        {
            //Given
            var dictionary = new HashtableDictionary<int, int>()
            {
                { 0, 100},
                {1, 100 },
                {3, 100 },
                {5, 100 },
                {16, 100 }
            };
            //When
            dictionary.Add(2, 100);
            //Then
            Assert.True(dictionary.ContainsKey(1));
            Assert.True(dictionary.ContainsKey(2));
            Assert.Equal(6, dictionary.Count);
        }

    }
}
