using System;
using System.Collections.Generic;
using System.IO;
using libSedML;
using libSedML.SBML;
using org.COPASI;

namespace LibCopasi2SedML
{
    /// <summary>
    /// Copasi to sed ML converter.
    /// </summary>
    public class Copasi2SedMLConverter
    {
        /// <summary>
        /// Gets or sets the copasi file.
        /// </summary>
        /// <value>
        /// The copasi file.
        /// </value>
        public string CopasiFile { get; set; }
        /// <summary>
        /// Gets or sets the output file.
        /// </summary>
        /// <value>
        /// The output file.
        /// </value>
        public string OutputFile { get; set; }
        /// <summary>
        /// Gets or sets the sed M.
        /// </summary>
        /// <value>
        /// The sed M.
        /// </value>
        public SedMLInfo SedML { get; set; }

        private readonly Dictionary<string, string> names = new Dictionary<string, string>();
        private readonly Dictionary<string, VariableType> types = new Dictionary<string, VariableType>();
        private readonly List<string> dataIds = new List<string>();

        /// <summary>
        /// Gets the SBML identifier.
        /// </summary>
        /// <returns>
        /// The SBML identifier.
        /// </returns>
        /// <param name='model'>
        /// Model.
        /// </param>
        /// <param name='channel'>
        /// Channel.
        /// </param>
        /// <param name='type'>
        /// Type.
        /// </param>
        /// <param name='name'>
        /// Name.
        /// </param>
        static string GetSBMLId(CModel model, CPlotDataChannelSpec channel, out VariableType type, out string name)
        {
            var path = channel.getString();
            var last = path.Substring(path.LastIndexOf(",", StringComparison.Ordinal) + 1);
            var first = path.Substring(0, path.LastIndexOf(",", StringComparison.Ordinal));

            type = VariableType.Unknown;
            name = "unknown";

            if (last.EndsWith("Rate") || last.EndsWith("ParticleNumber"))
                return null;

            if (model.getCN().getString() == first)
            {
                type = VariableType.Time;
                name = "Time";
                return "time";
            }

            for (int i = 0; i < model.getMetabolites().size(); i++)
            {
                var current = model.getMetabolite((uint)i);
                if (current.getCN().getString() == first)
                {
                    type = VariableType.Species;
                    name = current.getObjectDisplayName();
                    return current.getSBMLId();
                }
            }

            for (int i = 0; i < model.getCompartments().size(); i++)
            {
                var current = model.getCompartment((uint)i);
                if (current.getCN().getString() == first)
                {
                    type = VariableType.Compartment;
                    name = current.getObjectDisplayName();
                    return current.getSBMLId();
                }
            }

            for (int i = 0; i < model.getModelValues().size(); i++)
            {
                var current = model.getModelValue((uint)i);
                if (current.getCN().getString() == first)
                {
                    type = VariableType.Parameter;
                    name = current.getObjectDisplayName();
                    return current.getSBMLId();
                }
            }

            for (int i = 0; i < model.getReactions().size(); i++)
            {
                var current = model.getReaction((uint)i);
                if (current.getCN().getString() == first)
                {
                    type = VariableType.Reaction;
                    name = current.getObjectDisplayName();
                    return current.getSBMLId();
                }
            }

            return null;
        }
        /// <summary>
        /// Creates the plot.
        /// </summary>
        /// <returns>
        /// The plot.
        /// </returns>
        /// <param name='dataModel'>
        /// Data model.
        /// </param>
        /// <param name='current'>
        /// Current.
        /// </param>
        Plot2D CreatePlot(CCopasiDataModel dataModel, CPlotSpecification current)
        {
            var logX = current.isLogX();
            var logY = current.isLogY();
            var name = current.getTitle();
            var plot = new Plot2D
            {
                LogX = logX,
                LogY = logY,
                Name = name
            };
            for (int j = 0; j < current.getNumPlotItems(); j++)
            {
                var item = current.getItem(j);
                var data = new List<string>();
                var addItems = true;
                for (int k = 0; k < item.getChannels().Count; k++)
                {
                    var channel = item.getChannels()[k];
                    VariableType type;
                    string channelName;
                    var sbmlId = GetSBMLId(dataModel.getModel(), channel, out type, out channelName);
                    if (type == VariableType.Unknown || string.IsNullOrWhiteSpace(sbmlId))
                    {
                        addItems = false;
                        break;
                    }
                    data.Add(sbmlId);
                    names[sbmlId] = channelName;
                    types[sbmlId] = type;
                    if (!dataIds.Contains(sbmlId))
                        dataIds.Add(sbmlId);
                }
                if (addItems && data.Count == 2)
                {
                    plot.ListOfCurves.Add(new Curve
                    {
                        XDataReference = data[0],
                        YDataReference = data[1]
                    });
                }
            }
            return plot;
        }

        /// <summary>
        /// Creates the data generators.
        /// </summary>
        /// <param name='info'>
        /// Info.
        /// </param>
        void CreateDataGenerators(SedMLInfo info)
        {
            info.UpdateStores();
            foreach (var item in dataIds)
            {
                info.ListOfDataGenerators.Add(new DataGenerator
                {
                    Id = item,
                    Name = names[item],
                    ListOfVariables = new List<Variable>
                        {
						new Variable ("v", "task1", SBMLLanguage.GetXPath (types [item], item))
					},
                    MathML = "v"
                }
                );
            }
        }

        /// <summary>
        /// Convert the specified outFile.
        /// </summary>
        /// <param name='outFile'>
        /// Out file.
        /// </param>
        public void Convert(string outFile)
        {
            OutputFile = outFile;

            var path = Path.GetDirectoryName(OutputFile);
            var baseName = Path.GetFileNameWithoutExtension(OutputFile);

            if (path == null)
            {
                path = Environment.CurrentDirectory;
            }

            var sbmlFile = Path.Combine(path, baseName + "-sbml.xml");

            CCopasiRootContainer.init();
            var dataModel = CCopasiRootContainer.addDatamodel();
            dataModel.loadModel(CopasiFile);

            // write sbml model
            dataModel.exportSBML(sbmlFile, true);

            // construct SED-ML
            SedML = new SedMLInfo();
            SedML.ListOfModels.Add(new Model(
                                       "model1", sbmlFile));

            SedML.ListOfModels[0].ModelContent = File.ReadAllText(sbmlFile);

            var trajectory = (CTrajectoryTask)dataModel.getTask("Time-Course");
            if (trajectory != null)
            {
                var problem = (CTrajectoryProblem)trajectory.getProblem();
                var method = (CTrajectoryMethod)trajectory.getMethod();
                var startTime = problem.getOutputStartTime();
                var steps = (int)problem.getStepNumber();
                var initial = dataModel.getModel().getInitialTime();
                var endTime = startTime + steps * problem.getStepSize();

                var kisao = method.getSubType() == CCopasiMethod.deterministic
                                ? Algorithm.DeterministicAlgorithm
                                : Algorithm.StochasticAlgorithm;

                var simulation = new UniformTimeCourse("sim1", initial, startTime, endTime, steps)
                    {
                        Algorithm = kisao
                    };

                SedML.ListOfSimulations.Add(simulation);

                SedML.ListOfTasks.Add(new Task("task1", "sim1", "model1"));

                for (int i = 0; i < dataModel.getPlotDefinitionList().size(); i++)
                {
                    var current = dataModel.getPlotSpecification((uint)i);
                    var plot = CreatePlot(dataModel, current);

                    if (plot.ListOfCurves.Count > 0)
                        SedML.ListOfOutputs.Add(plot);
                }

                CreateDataGenerators(SedML);
            }
            SedML.FixCommonErrors();

        }

        /// <summary>
        /// Saves to.
        /// </summary>
        /// <param name='outFile'>
        /// Out file.
        /// </param>
        public void SaveTo(string outFile)
        {
            if (SedML == null) // not yet converted 
                Convert(outFile);

            if (SedML == null)  // conversion failed
                return;

            var extension = Path.GetExtension(outFile);
            if (extension != null && extension.EndsWith("sedx"))
                SedML.WriteToArchive(outFile);
            else
                SedML.WriteTo(outFile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LibCopasi2SedML.Copasi2SedMLConverter"/> class.
        /// </summary>
        /// <param name='copasiFile'>
        /// Copasi file.
        /// </param>
        public Copasi2SedMLConverter(string copasiFile)
        {
            CopasiFile = copasiFile;
        }
    };
}
