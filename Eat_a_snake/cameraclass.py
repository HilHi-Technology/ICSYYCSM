import pygame, sys, random, math
from pygame.locals import *
from locals import *
import hunterclass, snakeclass, items

class Camera():
    def __init__(self, window, hunter):
        self.window = window
        self.hunter = hunter
        self.screen = window.get_rect()
        self.screen.center = hunter.rect.center
        self.slack = 100

    #Take a true position and convert it to camera coordinates
    def convert(self, rect):
        offx, offy = self.screen.topleft
        rect = rect.copy()
        rect.x -= self.screen.x
        rect.y -= self.screen.y
        return rect


    def draw(self, sprites):
        surface_blit = self.window.blit
        for spr in sprites:
            surface_blit(spr.image, self.convert(spr.rect))

    def drawsingle(self, sprite):
        self.window.blit(sprite.image, self.convert(sprite.rect))

    def update(self):
        if self.screen.center[0] - self.hunter.rect.center[0] > self.slack:
            self.screen.center = (self.hunter.rect.center[0] + self.slack, self.screen.center[1])
        if self.screen.center[1] - self.hunter.rect.center[1] > self.slack:
            self.screen.center = (self.screen.center[0], self.hunter.rect.center[1] + self.slack)
            
        if -self.screen.center[0] + self.hunter.rect.center[0] > self.slack:
            self.screen.center = (self.hunter.rect.center[0] - self.slack, self.screen.center[1])
        if -self.screen.center[1] + self.hunter.rect.center[1] > self.slack:
            self.screen.center = (self.screen.center[0], self.hunter.rect.center[1] - self.slack)




        if self.screen.x < 0:
            self.screen.x = 0
        if self.screen.y < 0:
            self.screen.y = 0
        if self.screen.bottomright[0] > DOMAIN['x']:
            self.screen.bottomright = [DOMAIN['x'], self.screen.bottomright[1]]
        if self.screen.bottomright[1] > DOMAIN['y']:
            self.screen.bottomright = [self.screen.bottomright[0], DOMAIN['y']]



