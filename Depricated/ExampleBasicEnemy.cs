using Godot;
using System;

public partial class ExampleBasicEnemy : CreatureBase
{
	
	
    public RichTextLabel EnemyStateLabel;
    public RichTextLabel EnemyMoveStateLabel;
    public RichTextLabel EnemyJumpStateLabel;
    public RichTextLabel EnvironmentalStateLabel;
    public RichTextLabel EnemyAttackStateLabel;
    public RichTextLabel EnemyXSpeedLabel;
    public RichTextLabel EnemyYSpeedLabel;
    public AnimationPlayer EnemyAnimations;


    // starting off we set all the labels and get all the enemy components we need.
    public override void _Ready()
    {
        CreatureObject = this;
        CollisionShape = GetNode<CollisionShape2D>("Body");
        Sprite = GetNode<Sprite2D>("Sprite2D");
        Animations = GetNode<AnimationPlayer>("AnimationPlayer");
        _Enemy_text_helper();
        EnemyMoveStateLabel = GetNode<RichTextLabel>("EnemyMoveStateLabel");
        _move_text_helper();
        EnemyJumpStateLabel = GetNode<RichTextLabel>("EnemyJumpStateLabel");
        _jump_text_helper();
        EnvironmentalStateLabel = GetNode<RichTextLabel>("EnvironmentalStateLabel");
        _environmental_text_helper();
        EnemyAttackStateLabel = GetNode<RichTextLabel>("EnemyAttackStateLabel");
        _attack_text_helper();
        EnemyXSpeedLabel = GetNode<RichTextLabel>("XSpeed");
        EnemyYSpeedLabel = GetNode<RichTextLabel>("YSpeed");
    }
   

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
        

        MoveAndSlide();
	}



    private void _jump_text_helper()
    {
        EnemyJumpStateLabel.Text = "Jump State: " + js.ToString();
    }
    private void _Enemy_text_helper()
    {
        EnemyStateLabel.Text = "Enemy State: " + ds.ToString();
    }
    private void _environmental_text_helper()
    {
        EnvironmentalStateLabel.Text = "Environmental State: " + es.ToString();
    }
    private void _move_text_helper()
    {
        EnemyMoveStateLabel.Text = "Move State: " + ms.ToString();
    }
    private void _attack_text_helper()
    {
        EnemyAttackStateLabel.Text = "Attack State: " + ast.ToString();
    }
    private void _EnemyXSpeed_helper()
    {
        EnemyXSpeedLabel.Text = "X: " + this.GetRealVelocity().X.ToString();
    }
    private void _EnemyYSpeed_helper()
    {
        EnemyYSpeedLabel.Text = "Y: " + this.GetRealVelocity().Y.ToString();
    }
}
