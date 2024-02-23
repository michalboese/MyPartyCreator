import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SurveyDto } from '../interfaces/survey-dto';
import { ChoiceDto } from '../interfaces/choice-dto';
import { SurveyVoteDto } from '../interfaces/survey-vote-dto';
import { SurveyVoteSendDto } from '../interfaces/survey-vote-send-dto';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class SurveyService {
  private Url = environment.apiUrl + 'Survey/';
  constructor(private http: HttpClient) {}

  addSurvey(surveyData: SurveyDto) {
    return this.http.post<SurveyDto>(`${this.Url}addSurvey`, surveyData);
  }

  getAllSurveysOfEvent(eventId: string) {
    return this.http.get<SurveyDto[]>(
      `${this.Url}getAllSurveysOfEvent/${eventId}`
    );
  }

  addChoice(choiceData: ChoiceDto) {
    return this.http.post<ChoiceDto>(`${this.Url}addChoice`, choiceData);
  }

  removeChoice(choiceId: string) {
    return this.http.delete<ChoiceDto>(`${this.Url}removeChoice/${choiceId}`);
  }

  getVotesFromSurvey(surveyId: string) {
    return this.http.get<SurveyVoteDto[]>(
      `${this.Url}getVotesFromSurvey/${surveyId}`
    );
  }

  addVote(surveyVoteData: SurveyVoteSendDto) {
    return this.http.post<SurveyVoteDto>(`${this.Url}addVote`, surveyVoteData)
      .pipe(
        catchError((error: any) => {
          console.error('Błąd podczas dodawania głosu:', error);
          throw error; // Rzutuj błąd ponownie, aby przekazać go do komponentu
        })
      );
  }

  removeVote(voteId: string) {
    return this.http.delete<SurveyVoteDto>(`${this.Url}removeVote/${voteId}`);
  }

  deleteSurvey(surveyId: string) {
    return this.http.delete<SurveyDto>(`${this.Url}deleteSurvey/${surveyId}`);
  }
}
