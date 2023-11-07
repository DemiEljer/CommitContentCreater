## CommitContentCreater v 1.0.0
Утилита для генерации тела коммита на основе информации, указанной в коде, с помощью специальной разметки. Также, она позволяет вести файл историчности, согласуя номера версий.

### Пример верстки в коде

```C
//~ - Одиночная строка

// Данная нотация позволяет задать модификацию номера версии
// Последняя версия из файла "history.txt" будет изменена на указанные значения в каждой части версии (v1.0.0 -> v1.0.1) 

//~ v0.0.1

/*~
    - Многоcтрочный комментарий
    -- С указанием вложенности
    -- Второго уровня
    --- Каждый дефис добавляет один уровень вложенности
    --- С помощью табуляции
*/
```

### Аргументы утилиты

-g [path]        : Сгенерировать файл истории из лога git (...> git log > log.txt);

-h               : Выводит справочную информацию по утилите;

-v               : Вывести текущую версию проекта;

-c               : Флаг очистка проекта от строк с описанием коммита;

-e [extention]   : Указание допустимого расширения файла для поиска коммита (-e h -e c);

-f [path]        : Указание конкретного файла для анализа;

-u			     : Обновить утилиту;

Path             : Путь к директории указывается просто как строка.

### Пример
``` CommitContentCreater.exe -c -e h -e c D:\Projects\TMS\Multiplex_Lipgart\MUX_Master_STM_Electro_52LV ```

### Используемые сокращения и специальные символы
Добавлена возможность сокращенной записи ключевых слов.

```
    #f,  #F  -> [FIX]
    #m,  #M  -> [MODIFY]
    #e,  #E  -> [EVENT]
    #u,  #U  -> [UPDATE]
    #a,  #A  -> [ADD]
    #i,  #I  -> [INFORMATION]
    #n,  #N  -> [NOTE]
    #c,  #C  -> [CORRECTION]
    #im, #IM -> [IMPROVEMENT]
```
Пример
```C
//~ - #E Результаты командировки
//~ - #F Исправлено поведение дворников в случае возникновения ситуации рассинхронизации
```
Результат
```
- [EVENT] Результаты командировки
- [FIX] Исправлено поведение дворников в случае возникновения ситуации рассинхронизации
```

Реализована возможность создания строк заголовков, к которым не применяюется алгоритм выравнивания вложенности

```
    #!
```

Поддерживается возможность задания позиции строки коммита

```
    // #[Number]
    #1 Первая строка
    #2 Вторая строка
    #3 Третья строка
```

### Результат (commit.txt)
```
v 1.0.1

- Одиночная строка
- Многострочный комментарий
    - С указанием вложенности
    - Второго уровня
        - Каждый дефис добавляет один уровень вложенности
        - С помощью табуляции
```
### Установка
Необходимо скачать и распаковать архив "Install.rar" из последнего релиза.
### Примечания
Используется утилита [UniversalUpdater](https://github.com/DemiEljer/UniversalUpdater/releases/tag/v1.0.0) для автоматического обновления.
