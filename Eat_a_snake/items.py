import pygame, sys, random, math, eat_a_snake, time, hunterclass, snakeclass
from pygame.locals import *
from locals import *

class InventoryItem(pygame.sprite.Sprite):
    def __init__(self, image):
        self.active = 0
        pygame.sprite.Sprite.__init__(self)
        self.image = pygame.image.load(image).convert()
        self.image.set_colorkey(self.image.get_at((0,0)))
        self.rect = self.image.get_rect()
        self.rect.x = random.randint(50, DOMAIN['x'])
        self.rect.y = random.randint(50, DOMAIN['y'])
        self.movemod = 0
        self.target = None
    def setTarget(self, target):
        self.target = target
    def setMovementMod(self, mod):
        self.movemod = mod
    def getMovementMod(self):
        return self.movemod
    def use(self):
        print('Action not defined')


class Leaf(InventoryItem):
    def __init__(self):
        InventoryItem.__init__(self, 'assets/images/leaf.png')
        self.setMovementMod(5)  #NOTE: already have leaf.setMovementMod(5) in eat_a_snake.py ln65
        self.allowed_target_types = (hunterclass.Hunter,)
    def use(self, target):
        if type(target) not in self.allowed_target_types:
            self.kill()
            return
        self.rect.bottomright = (0,0) # Hide the sprite when item is used.
        self.setTarget(target)
        self.active = True
        self.sound = pygame.mixer.Sound("assets/audio/zOOOOOOoooOOOOOm.ogg")
        self.endtime = time.time() + self.sound.get_length()
        self.sound.play()
        self.target.movement_speed += self.movemod # Up the target (probably the hunter's) speed
    def update(self):
        # Check for active and duration. Delete at the end because leaves are consumable.
        if self.active and self.endtime < time.time():
            # Reduce the hunter's movement speed before we destroy ourself
            self.target.movement_speed -= self.movemod
            self.kill()


class FireEgg(InventoryItem):
    def __init__(self):
        InventoryItem.__init__(self, 'assets/images/fireegg.png')
        self.duration = 3
        self.firetimer = 0
        self.allowed_target_types = (hunterclass.Hunter, snakeclass.Snake)
    def use(self, target):
        if type(target) not in self.allowed_target_types:
            self.kill()
            return
        self.setTarget(target)
        self.active = True
        self.sound = pygame.mixer.Sound("assets/audio/volcanosplode.ogg")
        self.endtime = time.time() + self.duration
        self.sound.set_volume(1.0)
        self.sound.play(maxtime=self.duration*1000) #Play time is in milliseconds
    def update(self):
        if self.active:
            if self.firetimer < time.time():
                fireball = Fireball(self.target.rect.topleft, self.target.direction, self.target.movement_speed, self.target) 
                projectiles.add(fireball)
                self.firetimer = time.time() + 0.45
            if self.endtime < time.time():
                self.kill()


class Fireball(pygame.sprite.Sprite):
    def __init__(self, origin, direction, fireball_speed, owner):
        pygame.sprite.Sprite.__init__(self)
        self.owner = owner
        self.fireball_speed = fireball_speed
        self.direction = direction
        if type(owner) == snakeclass.Snake:
            self.image = pygame.image.load('assets/images/green-fireball.png').convert()
        else:
            self.image = pygame.image.load('assets/images/fireball.png').convert()
        self.image.set_colorkey(self.image.get_at((0,0)))
        if direction == RIGHT:
            self.image = pygame.transform.rotate(self.image,180)
        self.rect = self.image.get_rect()
        self.rect.topleft = origin
        self.duration = 5
        self.target = None
    def update(self):
        if self.direction == 0:
            self.direction = LEFT
        if self.target:
            #burn the target
            self.rect.center = self.target.rect.center
            self.rect.x += random.choice((-10,0,10))
            if time.time() > self.endtime:
                self.target.status['onfire'] = False
                self.target.movement_speed += 2
                self.kill()
        elif self.rect.x < 0 or self.rect.y < 0 or self.rect.x > DOMAIN['x'] or self.rect.y > DOMAIN['y']:
            self.kill()
        else:
            #fly around
            self.rect.x += (self.direction * self.fireball_speed) + (10 * self.direction) #fireball speed
            
    def use(self, target):
        if target == self.owner:
            return False
        self.target = target
        self.image = pygame.transform.rotate(self.image,-270)
        target.movement_speed -= 2
        target.status['onfire'] = True
        self.endtime = time.time() + self.duration
        return True #Successfully used let the calling funciton know.


class AimedFireball(Fireball):
    def __init__(self, origin, direction, aimed_at):
        Fireball.__init__(self, origin, direction, 0, 0)
        self.aimed_at = aimed_at
        self.speed = 4
        if self.rect.x > self.aimed_at.rect.x:
            self.direction = LEFT
        else:
            self.direction = RIGHT
        self.rise = float(self.rect.y - self.aimed_at.rect.y)
        self.run = float(self.rect.x - self.aimed_at.rect.x)
        # self.slope = self.rise/self.run
        self.angle = (math.atan2(self.run, self.rise) + math.pi/2) 
        self.image = pygame.image.load('assets/images/blue-fireball.png').convert()
        self.image.set_colorkey(self.image.get_at((0,0)))
        self.image = pygame.transform.rotate(self.image, 180 + math.degrees(self.angle))

    def update(self):
        if self.target:
            #burn the target
            self.rect.center = self.target.rect.center
            self.rect.x += random.choice((-10,0,10))
            if time.time() > self.endtime:
                self.target.movement_speed += 2
                self.target.status['onfire'] = False
                self.kill()
        elif self.rect.x < 0 or self.rect.y < 0 or self.rect.x > DOMAIN['x'] or self.rect.y > DOMAIN['y']:
            self.kill()
        else:
            #fly around
            self.rect.x += math.floor(self.speed * math.cos(self.angle))
            self.rect.y += math.floor(-1 * self.speed * math.sin(self.angle)) # Negative -1 because y coordinates going up moves down the screen.
            self.speed = self.speed**1.02


class FireBloom(InventoryItem):
    def __init__(self):
        InventoryItem.__init__(self, 'assets/images/firebloom.bmp')
        self.duration = 10
        self.chargetime = 1
        self.firetimer = 0
        self.allowed_target_types = (hunterclass.Hunter,)
    def use(self, target):
        if type(target) not in self.allowed_target_types:
            self.kill()
            return
        self.setTarget(target)
        self.active = True
        self.sound = pygame.mixer.Sound("assets/audio/door.wav")
        self.endtime = time.time() + self.duration
        self.starttime = time.time() + self.chargetime
        self.sound.set_volume(1.0)
        self.sound.play(maxtime=self.duration*1000) #Play time is in milliseconds
        self.rect.topleft = target.rect.topleft
        stationary_objects.add(self)
    def update(self):
        if self.active:
            if time.time() > self.starttime:
                if self.firetimer < time.time():
                    fireball = AimedFireball(self.rect.topleft, -1, self.target)
                    projectiles.add(fireball)
                    self.firetimer = time.time() + 0.45
            if self.endtime < time.time():
                self.kill()


class Fork(InventoryItem):
    shoot = 0   #Member variable, because self.shoot won't let me refer to it with keydown commands over in eat_a_snake.py
    def __init__(self):
        InventoryItem.__init__(self, 'assets/images/fork.png')
        self.rect.x = DOMAIN['x']/2 + 200
        self.rect.y = DOMAIN['y']/2 + 200
        self.duration = 3
        self.allowed_target_types = (hunterclass.Hunter,)
    def use(self, target):
        if type(target) not in self.allowed_target_types:
            self.kill()
            return
        self.setTarget(target)
        self.active = True
        self.sound = pygame.mixer.Sound("assets/audio/hunt.wav")
        self.endtime = time.time() + self.duration
        self.sound.set_volume(1.0)
        self.sound.play(maxtime=self.duration*1000) #Play time is in milliseconds
        hunterclass.Hunter.wieldingMasterFork += 1   #Once the fork is used, tells hunter to wield master fork
    def update(self):
        if self.active:
            if Fork.shoot:
                bubble = Bubble(self.target.rect.topleft, self.target.direction, self.target.movement_speed, self.target) 
                projectiles.add(bubble)
                Fork.shoot = 0 #So that bubbles wont be shot infinitely
                if hunterclass.Hunter.wieldingMasterFork == 2:
                    bubble = Bubble(self.target.rect.topleft, -(self.target.direction), self.target.movement_speed, self.target) 
                    projectiles.add(bubble)


class Bubble(pygame.sprite.Sprite):
    def __init__(self, origin, direction, bubble_speed, owner):
        pygame.sprite.Sprite.__init__(self)
        self.owner = owner
        self.bubble_speed = bubble_speed
        self.direction = direction
        self.image = pygame.image.load('assets/images/bubble.png').convert()
        self.image.set_colorkey(self.image.get_at((0,0)))
        self.rect = self.image.get_rect()
        self.rect.topleft = origin
        self.duration = 5
        self.target = None
    def update(self):
        if self.direction == 0:
            self.direction = -1
        if self.target:
            #burn the target
            self.rect.center = self.target.rect.center
            self.rect.x += random.choice((-10,0,10))
            if self.target.status['onfire'] == True and type(self.target) == snakeclass.BabySnake:
                self.target.kill()
            if time.time() > self.endtime:
                self.target.movement_speed += 2
                if self.target.status['onfire'] == True and type(self.target) == snakeclass.Snake:
                    self.target.kill()
                self.kill()
        elif self.rect.x < 0 or self.rect.y < 0 or self.rect.x > DOMAIN['x'] or self.rect.y > DOMAIN['y']:
            self.kill()
        else:
            #fly around
            self.rect.x += (self.direction * self.bubble_speed) + (10 * self.direction) #fireball speed
    def use(self, target):
        if target == self.owner:
            return 0
        self.target = target
        self.image = pygame.transform.rotate(self.image,-270)
        target.movement_speed -= 2
        self.endtime = time.time() + self.duration
        return True #Successfully used let the calling function know.
