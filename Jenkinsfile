pipeline {
  agent any
  stages {
    stage('Get Git Files') {
      steps {
        git(url: 'git@github.com:DevSimp2023/JenkinsTest.git', branch: 'master')
      }
    }

    stage('Get Enviroment') {
      steps {
        sh 'ls ${UNITY_PATH}'
        sh 'echo ${TARGET}'
      }
    }

  }
  environment {
    UNITY_PATH = 'D:\\DevTools\\UnityEditor\\2021.3.20f1c1\\Editor'
    TARGET = 'Android'
  }
}