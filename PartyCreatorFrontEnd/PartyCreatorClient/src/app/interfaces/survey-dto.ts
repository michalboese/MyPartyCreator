import { ChoiceDto } from './choice-dto';

export interface SurveyDto {
  surveyId: number;
  eventId: number;
  question: string;
  choices: ChoiceDto[];
}
