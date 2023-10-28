## CommitContentCreater v 1.0.0
Утилита для генерации тела коммита на основе информации, указанной в коде, с помощью специальной разметки. Также, она позволяет вести файл историчности, согласуя номера версий.

### Пример верстки в колде

```C
//~ - Одиночная строка

// Данная нотация позволяет задать модификацию номера версии
// Последняя версия из файла "history.txt" будет изменена на указанные значения в каждой части версии (v1.0.0 -> v1.0.1) 

//~ v0.0.1

/*~
    - Многочтрочный комментарий
    -- С указанием вложенности
    -- Второго уровня
    --- Каждый дефис добавляет один уровень вложенности
    --- С помощью табуляции
*/
```

### Аргументы утилиты
-h               : Выводит справочную ифнормацию по утилите

-c               : Флаг очистка проекта от строк с описанием коммита

-e <extention>   : Указание допустимого расширения файла для поиска коммита (-e h -e c)

Path           : Путь к директории указывается просто как строка

### Пример
``` CommitContentCreater.exe -c -e h -e c D:\Projects\TMS\Multiplex_Lipgart\MUX_Master_STM_Electro_52LV ```

### Используемые сокращения
Добавлена возможность сокращенной записи ключевых слов.

```
    #f, #F -> [FIX]
    #m, #M -> [MODIFY]
    #e, #E -> [EVENT]
    #u, #U -> [UPDATE]
    #a, #A -> [ADD]
    #i, #I -> [INFORMATION]
    #n, #N -> [NOTE]
```

### Результат (commit.txt)
```
v 1.0.1

- Одиночная строка
- Многочтрочный комментарий
    - С указанием вложенности
    - Второго уровня
        - Каждый дефис добавляет один уровень вложенности
        - С помощью табуляции
```
