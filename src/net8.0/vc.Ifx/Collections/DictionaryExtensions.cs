using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;

using vc.Ifx;
using vc.Ifx.Collections;

namespace vc.Ifx.Collections;

public static class DictionaryExtensions
{
    /// <summary>
    /// Gets a value from a dictionary or returns a default value if the key doesn't exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to search</param>
    /// <param name="key">The key to find</param>
    /// <param name="defaultValue">The default value to return if the key is not found</param>
    /// <returns>The value associated with the key or the default value</returns>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default!)
    {
        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Gets a value from a dictionary or computes it if the key doesn't exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to search</param>
    /// <param name="key">The key to find</param>
    /// <param name="valueFactory">A function that computes the value if the key is not found</param>
    /// <returns>The value associated with the key or the computed value</returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        ArgumentNullException.ThrowIfNull(valueFactory, nameof(valueFactory));

        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }
        value = valueFactory(key);
        dictionary[key] = value;
        return value;
    }

    /// <summary>
    /// Adds or updates a value in the dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to modify</param>
    /// <param name="key">The key to add or update</param>
    /// <param name="addValue">The value to add if the key doesn't exist</param>
    /// <param name="updateValueFactory">A function to generate an updated value based on the key and existing value</param>
    /// <returns>The new value in the dictionary</returns>
    public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        ArgumentNullException.ThrowIfNull(updateValueFactory, nameof(updateValueFactory));

        if (dictionary.TryGetValue(key, out var existingValue))
        {
            var newValue = updateValueFactory(key, existingValue);
            dictionary[key] = newValue;
            return newValue;
        }
        dictionary[key] = addValue;
        return addValue;
    }

    /// <summary>
    /// Adds or updates a value in the dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to modify</param>
    /// <param name="key">The key to add or update</param>
    /// <param name="addValueFactory">A function to generate a value to add if the key doesn't exist</param>
    /// <param name="updateValueFactory">A function to generate an updated value based on the key and existing value</param>
    /// <returns>The new value in the dictionary</returns>
    public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        ArgumentNullException.ThrowIfNull(addValueFactory, nameof(addValueFactory));
        ArgumentNullException.ThrowIfNull(updateValueFactory, nameof(updateValueFactory));

        if (dictionary.TryGetValue(key, out var existingValue))
        {
            var newValue = updateValueFactory(key, existingValue);
            dictionary[key] = newValue;
            return newValue;
        }
        var addValue = addValueFactory(key);
        dictionary[key] = addValue;
        return addValue;
    }

    /// <summary>
    /// Converts a dictionary to an immutable dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to convert</param>
    /// <returns>An immutable version of the dictionary</returns>
    public static IImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) where TKey 
        : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        return dictionary.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Converts a dictionary to a read-only dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to convert</param>
    /// <returns>A read-only version of the dictionary</returns>
    public static ReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) where TKey 
        : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        return new ReadOnlyDictionary<TKey, TValue>(dictionary);
    }

    /// <summary>
    /// Merges two dictionaries into a new dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionaries</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionaries</typeparam>
    /// <param name="first">The first dictionary</param>
    /// <param name="second">The second dictionary</param>
    /// <param name="conflictResolver">Optional function to resolve conflicts when keys exist in both dictionaries</param>
    /// <returns>A new dictionary containing all keys and values from both input dictionaries</returns>
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second, Func<TKey, TValue, TValue, TValue>? conflictResolver = null) where TKey 
        : notnull
    {
        ArgumentNullException.ThrowIfNull(first, nameof(first));
        ArgumentNullException.ThrowIfNull(second, nameof(second));
        var result = new Dictionary<TKey, TValue>(first);
        foreach (var kvp in second)
        {
            if (result.TryGetValue(kvp.Key, out var existingValue))
            {
                if (conflictResolver != null)
                {
                    result[kvp.Key] = conflictResolver(kvp.Key, existingValue, kvp.Value);
                }
                else
                {
                    result[kvp.Key] = kvp.Value; // Second dictionary wins by default
                }
            }
            else
            {
                result.Add(kvp.Key, kvp.Value);
            }
        }
        return result;
    }

    /// <summary>
    /// Applies a transformation function to each value in a dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <typeparam name="TResult">The type of the result values</typeparam>
    /// <param name="dictionary">The dictionary to transform</param>
    /// <param name="valueSelector">A function to transform each value</param>
    /// <returns>A new dictionary with the same keys but transformed values</returns>
    public static Dictionary<TKey, TResult> TransformValues<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, Func<TValue, TResult> valueSelector) where TKey 
        : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        ArgumentNullException.ThrowIfNull(valueSelector, nameof(valueSelector));
        var result = new Dictionary<TKey, TResult>(dictionary.Count);
        foreach (var kvp in dictionary)
        {
            result.Add(kvp.Key, valueSelector(kvp.Value));
        }
        return result;
    }

    /// <summary>
    /// Filters a dictionary based on a predicate.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to filter</param>
    /// <param name="predicate">A function to test each key-value pair for a condition</param>
    /// <returns>A new dictionary containing only the elements that satisfy the condition</returns>
    public static Dictionary<TKey, TValue> Where<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, bool> predicate) where TKey 
        : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
        return dictionary
            .Where(kvp => predicate(kvp.Key, kvp.Value))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Creates a dictionary from an object's properties.
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <param name="obj">The object to convert to a dictionary</param>
    /// <returns>A dictionary with property names as keys and property values as values</returns>
    public static Dictionary<string, object?> ToDictionary<T>(this T obj) where T 
        : class
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        var dictionary = new Dictionary<string, object?>();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            dictionary[property.Name] = property.GetValue(obj);
        }
        return dictionary;
    }
    
    /// <summary>
    /// Checks if a dictionary is null or empty.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to check</param>
    /// <returns>True if the dictionary is null or empty; otherwise, false</returns>
    public static bool IsNullOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue>? dictionary)
    {
        return dictionary == null || dictionary.Count == 0;
    }

    /// <summary>
    /// Removes multiple keys from a dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to modify</param>
    /// <param name="keys">The keys to remove</param>
    /// <returns>The number of elements removed</returns>
    public static int RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        var count = 0;
        foreach (var key in keys)
        {
            if (dictionary.Remove(key))
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Tries to remove a key from the dictionary and returns its value.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to modify</param>
    /// <param name="key">The key to remove</param>
    /// <param name="value">The value associated with the key if found, default otherwise</param>
    /// <returns>True if the key was found and removed; otherwise, false</returns>
    public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));

        if (dictionary.TryGetValue(key, out value))
        {
            return dictionary.Remove(key);
        }
        value = default!;
        return false;
    }

    /// <summary>
    /// Tries to update a value for an existing key.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to modify</param>
    /// <param name="key">The key to update</param>
    /// <param name="newValue">The new value to set</param>
    /// <returns>True if the key was found and updated; otherwise, false</returns>
    public static bool TryUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue newValue)
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));

        if (!dictionary.ContainsKey(key)) 
            return false;
        dictionary[key] = newValue;
        return true;
    }

    /// <summary>
    /// Performs an action on each element in the dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to process</param>
    /// <param name="action">The action to perform on each element</param>
    public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Action<TKey, TValue> action)
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        foreach (var kvp in dictionary)
        {
            action(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Inverts a dictionary, using values as keys and keys as values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the original dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the original dictionary</typeparam>
    /// <param name="dictionary">The dictionary to invert</param>
    /// <returns>A new dictionary with keys and values swapped</returns>
    /// <exception cref="ArgumentException">Thrown when the original dictionary contains duplicate values.</exception>
    public static Dictionary<TValue, TKey> Invert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));

        var result = new Dictionary<TValue, TKey>(dictionary.Count);
        foreach (var kvp in dictionary)
        {
            if (result.ContainsKey(kvp.Value))
            {
                throw new ArgumentException("Dictionary cannot be inverted because it contains duplicate values");
            }
            result.Add(kvp.Value, kvp.Key);
        }
        return result;
    }

    /// <summary>
    /// Increments a numeric value in a dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <param name="dictionary">The dictionary to modify</param>
    /// <param name="key">The key to increment</param>
    /// <param name="increment">The increment value (default 1)</param>
    /// <returns>The new value after incrementing</returns>
    public static int IncrementValue<TKey>(this IDictionary<TKey, int> dictionary, TKey key, int increment = 1)
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));

        if (dictionary.TryGetValue(key, out var currentValue))
        {
            var newValue = currentValue + increment;
            dictionary[key] = newValue;
            return newValue;
        }

        dictionary[key] = increment;
        return increment;
    }

    /// <summary>
    /// Adds an item to a list value in a dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TListItem">The type of items in the list</typeparam>
    /// <param name="dictionary">The dictionary to modify</param>
    /// <param name="key">The key to add to</param>
    /// <param name="item">The item to add to the list</param>
    public static void AddToList<TKey, TListItem>(this IDictionary<TKey, List<TListItem>> dictionary, TKey key, TListItem item)
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));

        if (!dictionary.TryGetValue(key, out var list))
        {
            list = [];
            dictionary[key] = list;
        }
        list.Add(item);
    }
}