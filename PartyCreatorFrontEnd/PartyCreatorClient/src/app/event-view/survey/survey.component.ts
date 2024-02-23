import { Component, OnInit, Input } from '@angular/core';
import { SurveyService } from '../../services/survey.service';
import { SurveyDto } from '../../interfaces/survey-dto';
import { SurveyVoteSendDto } from '../../interfaces/survey-vote-send-dto';
import { RoleDto } from 'src/app/interfaces/role-dto';
import { EventService } from 'src/app/services/event.service';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatNativeDateModule } from '@angular/material/core';
import { NgToastService } from 'ng-angular-popup';

@Component({
  selector: 'app-survey',
  templateUrl: './survey.component.html',
  styleUrls: ['./survey.component.css'],
})
export class SurveyComponent implements OnInit {
  userRole: RoleDto = { id: 0, role: '' };
  surveys: SurveyDto[] = [];
  newSurveyForm: FormGroup;
  step = 0;
  surveyResults: Map<number, number[]> = new Map();

  selectedChoices: Map<number, number> = new Map();
  @Input() userId: number = 0;
  @Input() eventId = '';

  constructor(
    private surveyService: SurveyService,
    private eventService: EventService,
    private authService: AuthService,
    private toast: NgToastService,
    private fb: FormBuilder
  ) {
    this.newSurveyForm = this.fb.group({
      question: ['', Validators.required],
      choices: this.fb.array([
        this.fb.control('', Validators.required),
        this.fb.control('', Validators.required),
      ]),
    });
  }

  ngOnInit(): void {
    this.loadSurveys();
    this.loadUserRole();
  }

  loadUserRole(): void {
    this.eventService.getAccess(this.eventId).subscribe(
      (data) => (this.userRole = data),
      (error) => console.error('Error loading user role', error)
    );
  }

  loadSurveys(): void {
    if (!this.eventId) {
      console.error('Event ID is not provided for Survey Component');
      return;
    }

    this.surveyService.getAllSurveysOfEvent(this.eventId).subscribe({
      next: (data) => {
        console.log('Ankiety załadowane:', data);
        this.surveys = data;

        data.forEach((survey) => {
          this.surveyService
            .getVotesFromSurvey(survey.surveyId.toString())
            .subscribe({
              next: (votes) => {
                this.surveyResults.set(
                  survey.surveyId,
                  votes.map((vote) => vote.choiceId)
                );
              },
              error: (err) => console.error('Error loading survey votes', err),
            });
        });
      },
      error: (err) => console.error('Error loading surveys', err),
    });
  }

  addNewChoice(): void {
    const choices = this.newSurveyForm.get('choices') as FormArray;
    if (choices.length < 4) {
      choices.push(this.fb.control('', Validators.required));
    } else {
      this.toast.error({
        detail: 'ERROR',
        summary: 'Możesz dodać maksymalnie 4 opcje do wyboru!',
        duration: 3000,
      });
    }
  }

  createSurvey(): void {
    const formValue = this.newSurveyForm.value;
    const newSurvey: SurveyDto = {
      surveyId: 0, // lub inna logika do ustalania ID, jeśli to konieczne
      eventId: parseInt(this.eventId, 10),
      question: formValue.question,
      choices: formValue.choices.map((choiceText: string) => ({
        id: 0, // lub inna logika do ustalania ID, jeśli to konieczne
        surveyId: 0, // lub inna logika do ustalania ID, jeśli to konieczne
        choiceText: choiceText,
      })),
    };

    this.surveyService.addSurvey(newSurvey).subscribe({
      next: (data) => {
        console.log('Ankieta została dodana pomyślnie', data);
        this.toast.success({
          detail: 'SUKCES',
          summary: 'Dodano ankietę!',
          duration: 3000,
        });
        this.loadSurveys();
        this.newSurveyForm = this.fb.group({
          question: [''],
          choices: this.fb.array([this.fb.control(''), this.fb.control('')]),
        });
      },
      error: (err) => console.error('Błąd podczas dodawania ankiety', err),
    });
  }

  getChoicesControls() {
    return (this.newSurveyForm.get('choices') as FormArray).controls;
  }

  submitVote(surveyId: number): void {
    const choiceId = this.selectedChoices.get(surveyId);

    if (choiceId !== undefined) {
      const voteData: SurveyVoteSendDto = {
        id: 0,
        surveyId,
        userId: this.userId,
        choiceId,
      };
      this.surveyService.addVote(voteData).subscribe({
        next: () => {
          console.log('Vote submitted successfully');
          this.toast.success({
            detail: 'SUKCES',
            summary: 'Twój wybór został zapisany!',
            duration: 3000,
          });
          this.loadSurveys(); // Ponowne załadowanie ankiet po oddaniu głosu
        },
        error: (err) => {
          console.error('Error submitting vote', err);
          this.toast.error({
            detail: 'ERROR',
            summary: 'Już oddałeś głos w tej ankiecie, nie możesz go zmienić!',
            duration: 3000,
          });
        },
      });
    }
  }

  deleteSurvey(surveyId: number): void {
    this.surveyService.deleteSurvey(surveyId.toString()).subscribe({
      next: () => {
        this.toast.success({
          detail: 'SUKCES',
          summary: 'Ankieta została usunięta!',
          duration: 3000,
        });
        this.loadSurveys();
      },
      error: (err) => console.error('Błąd podczas usuwania ankiety', err),
    });
  }

  setStep(index: number) {
    this.step = index;
  }

  nextStep() {
    this.step++;
  }

  prevStep() {
    this.step--;
  }

  getChoiceVoteCount(surveyId: number, choiceId: number): number {
    const surveyResults = this.surveyResults.get(surveyId);
    if (surveyResults) {
      return surveyResults.filter((result) => result === choiceId).length;
    }
    return 0;
  }
  removeChoice(index: number): void {
    const choices = this.newSurveyForm.get('choices') as FormArray;
    if (choices.length > 2) {
      choices.removeAt(index);
    } else {
      this.toast.error({
        detail: 'ERROR',
        summary: 'Nie możesz usunąć wszystkich opcji!',
        duration: 3000,
      });
    }
  }
}
