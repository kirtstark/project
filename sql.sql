


CREATE DATABASE MEALS;
USE MEALS;


DROP TABLE IF EXISTS Meal;
CREATE TABLE Meal (
  name VARCHAR(25) PRIMARY KEY 
);

DROP TABLE IF EXISTS Category;
CREATE TABLE Category (
  name VARCHAR(25) PRIMARY KEY 
);

DROP TABLE IF EXISTS Source;
CREATE TABLE Source (
  name VARCHAR(25) PRIMARY KEY 
);

DROP TABLE IF EXISTS Season;
CREATE TABLE Season (
  name VARCHAR(25) PRIMARY KEY 
);

DROP TABLE IF EXISTS Cuisine;
CREATE TABLE Cuisine (
  name VARCHAR(255) PRIMARY KEY 
);

DROP TABLE IF EXISTS Day;
CREATE TABLE Day (
  name VARCHAR(25) PRIMARY KEY, 
  num INTEGER
);

DROP TABLE IF EXISTS Units;
CREATE TABLE Units (
  name VARCHAR(25) PRIMARY KEY
);

DROP TABLE IF EXISTS Menu;
CREATE TABLE Menu (
  item VARCHAR(25),
  category VARCHAR(25),
  meal VARCHAR(25),
  weekday VARCHAR(25),
  dayNum INT,
  PRIMARY KEY (item, meal, weekday),
  FOREIGN KEY (meal) REFERENCES Meal(name)
);


DROP TABLE IF EXISTS Entrees;
CREATE TABLE Entrees (
    name VARCHAR(255) PRIMARY KEY,
    cuisine VARCHAR(255),
    season VARCHAR(25),
    lastHad DATE,
    recipe VARCHAR(4000),
    popularity INT,
    meal VARCHAR(25),
    FOREIGN KEY (cuisine) REFERENCES Cuisine(name),
    FOREIGN KEY (season) REFERENCES Season(name),
    FOREIGN KEY (meal) REFERENCES Meal(name)
);

DROP TABLE IF EXISTS SideDishes;
CREATE TABLE SideDishes (
    name VARCHAR(255) PRIMARY KEY,
    cuisine VARCHAR(255),
    season VARCHAR(25),
    lastHad DATE,
    recipe VARCHAR(4000),
    popularity INT,
    meal VARCHAR(25),
    FOREIGN KEY (cuisine) REFERENCES Cuisine(name),
    FOREIGN KEY (season) REFERENCES Season(name),
    FOREIGN KEY (meal) REFERENCES Meal(name)
);

DROP TABLE IF EXISTS Deserts;
CREATE TABLE Deserts (
    name VARCHAR(255) PRIMARY KEY,
    cuisine VARCHAR(255),
    season VARCHAR(25),
    lastHad DATE,
    recipe VARCHAR(4000),
    popularity INT,
    FOREIGN KEY (cuisine) REFERENCES Cuisine(name),
    FOREIGN KEY (season) REFERENCES Season(name)
);

DROP TABLE IF EXISTS Ingredients;
CREATE TABLE Ingredients (
    name VARCHAR(255) PRIMARY KEY,
    category VARCHAR(25),
    source VARCHAR(25),
    staple BOOL,
    isle INT,
    FOREIGN KEY (category) REFERENCES Category(name),
    FOREIGN KEY (source) REFERENCES Source(name)
);



DROP TABLE IF EXISTS EntreeIngredients;
CREATE TABLE EntreeIngredients (
    name VARCHAR(255),
    amount DOUBLE,
    units VARCHAR(25),
    entreeName VARCHAR(255),
    PRIMARY KEY (name, entreeName),
    FOREIGN KEY (name) REFERENCES Ingredients(name)
);

DROP TABLE IF EXISTS SideDishIngredients;
CREATE TABLE SideDishIngredients (
    name VARCHAR(255),
    amount DOUBLE,
    units VARCHAR(25),
    sideDishName VARCHAR(255),
    PRIMARY KEY (name, sideDishName),
    FOREIGN KEY (name) REFERENCES Ingredients(name)
);

DROP TABLE IF EXISTS DesertIngredients;
CREATE TABLE DesertIngredients (
    name VARCHAR(255),
    amount DOUBLE,
    units VARCHAR(25),
    desertName VARCHAR(255),
    PRIMARY KEY (name, desertName),
    FOREIGN KEY (name) REFERENCES Ingredients(name)
);





DELIMITER $$

DROP PROCEDURE IF EXISTS ShoppingList$$

CREATE PROCEDURE ShoppingList()
BEGIN

(SELECT DISTINCT name, source, isle FROM Ingredients WHERE staple = true ORDER BY source, isle)
UNION
(SELECT name, source, isle FROM Ingredients WHERE name IN
(SELECT name FROM DesertIngredients, Menu WHERE desertName = Menu.item) OR name IN
(SELECT name FROM SideDishIngredients, Menu WHERE sideDishName = Menu.item) OR name IN
(SELECT name FROM EntreeIngredients, Menu WHERE entreeName = Menu.item))
ORDER BY source, isle;
END$$





DROP PROCEDURE IF EXISTS addIngredient$$

CREATE PROCEDURE addIngredient(IN name1 VARCHAR(255), IN category1 VARCHAR(25), IN source1 VARCHAR(25), IN staple1 BOOL, IN isle1 INTEGER)
BEGIN
	INSERT INTO Ingredients(name, category, source, staple, isle) VALUES (name1, category1, source1, staple1, isle1);

END $$

DELIMITER ;


insert into Units values('cups');
insert into Units values('tsp');
insert into Units values('TBL');
insert into Units values('pinch');
insert into Units values('ounces');
insert into Units values('pounds');
insert into Units values('quarts');


insert into Day values('Sunday', 0);
insert into Day values('Monday', 1);
insert into Day values('Tuesday', 2);
insert into Day values('Wednesday', 3);
insert into Day values('Thursday', 4);
insert into Day values('Friday', 5);
insert into Day values('Saturday', 6);


insert into Category values('grain');
insert into Category values('dairy');
insert into Category values('meat');
insert into Category values('poultry');
insert into Category values('sweets');



insert into source values('Fred Meyers');
insert into source values('Hagans');
insert into source values('Whole Foods');
insert into source values('Lucky Vitamins');


insert into Season values('Winter');
insert into Season values('Spring');
insert into Season values('Summer');
insert into Season values('Autumn');
insert into Season values('Cool');
insert into Season values('Hot');


insert into Cuisine values('Mexican');
insert into Cuisine values('Asian');
insert into Cuisine values('Italian');
insert into Cuisine values('Greek');
insert into Cuisine values('American');



insert into Meal values('Breakfast');
insert into Meal values('Lunch');
insert into Meal values('Dinner');
insert into Meal values('Snack');
insert into Meal values('Lunch or Dinner');
insert into Meal values('Any');

insert into ingredients values (name, category, source, staple(true or false), isle);


insert into ingredients values ('ing2', 'grain', 'Fred Meyers', true, 5);
insert into ingredients values ('ing3', 'meat', 'Hagans', false, 3);
insert into ingredients values ('ing4', 'dairy', 'Whole Foods', false, 2);
insert into ingredients values ('ing5', 'grain', 'Fred Meyers', true, 4);
insert into ingredients values ('ing6', 'grain', 'Fred Meyers', false, 0);
insert into ingredients values ('ing7', 'dairy', 'Hagans', true, 5);
insert into ingredients values ('ing8', 'grain', 'Whole Foods', true, 7);
insert into ingredients values ('ing9', 'grain', 'Lucky Vitamins', true, 13);

insert into ingredients values ('ing10', 'poultry', 'Fred Meyers', true, 5);
insert into ingredients values ('ing11', 'grain', 'Hagans', false, 5);
insert into ingredients values ('ing12', 'sweets', 'Whole Foods', false, 19);
insert into ingredients values ('ing13', 'sweets', 'Fred Meyers', true, 0);
insert into ingredients values ('ing14', 'grain', 'Fred Meyers', false, 4);
insert into ingredients values ('ing15', 'grain', 'Hagans', true, 5);
insert into ingredients values ('ing16', 'sweets', 'Whole Foods', true, 5);
insert into ingredients values ('ing17', 'grain', 'Lucky Vitamins', true, 6);

insert into ingredients values ('ing18', 'grain', 'Fred Meyers', true, 24);
insert into ingredients values ('ing19', 'meat', 'Hagans', false, 2);
insert into ingredients values ('ing20', 'dairy', 'Whole Foods', false, 95);
insert into ingredients values ('ing21', 'grain', 'Fred Meyers', true, 5);
insert into ingredients values ('ing22', 'grain', 'Fred Meyers', false, 8);
insert into ingredients values ('ing23', 'dairy', 'Hagans', true, 5);
insert into ingredients values ('ing24', 'grain', 'Whole Foods', true, 0);
insert into ingredients values ('ing25', 'grain', 'Lucky Vitamins', true, 5);

insert into ingredients values ('ing26', 'poultry', 'Fred Meyers', true, 5);
insert into ingredients values ('ing27', 'grain', 'Hagans', false, 3);
insert into ingredients values ('ing28', 'sweets', 'Whole Foods', false, 9);
insert into ingredients values ('ing29', 'sweets', 'Fred Meyers', true, 1);
insert into ingredients values ('ing30', 'grain', 'Fred Meyers', false, 7);
insert into ingredients values ('ing31', 'grain', 'Hagans', true, 10);
insert into ingredients values ('ing32', 'sweets', 'Whole Foods', true, 11);
insert into ingredients values ('ing33', 'grain', 'Lucky Vitamins', true, 12);




