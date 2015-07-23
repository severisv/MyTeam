using System.Collections.Generic;

namespace MyTeam.ViewModels.Training
{
    public class ShowTrainingsViewModel
    {

        public IEnumerable<Models.Domain.Training> Trainings { get; private set; }

        public ShowTrainingsViewModel(IEnumerable<Models.Domain.Training> trainings)
        {
            Trainings = trainings;
        }
    }
}